using LibGit2Sharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelaLib;
using VelaWeb.Server.Models;

namespace VelaWeb.Server.Git
{
    public delegate Task ChangedHandler(object sender, string[] files);
    public interface IGitRepositoryCommitWatcher : IDisposable
    {
        event EventHandler<Exception> Error;
        event ChangedHandler Changed;
        /// <summary>
        /// 文件夹消失事件
        /// </summary>
        event EventHandler FolderMiss;
        void Continue();
        bool IsBranchReset();
        Task PauseAsync();
        void Start();
    }

    public class DefaultGitRepositoryCommitWatcher : IGitRepositoryCommitWatcher
    {
        private readonly string _localRepositoryPath;
        private readonly string _gitHash;
        private readonly int _intervalSeconds;
        private readonly ILogger<IGitRepositoryCommitWatcher> _logger;
        public event ChangedHandler Changed;
        public event EventHandler<Exception> Error;
        public event EventHandler FolderMiss;
        ProjectCenter _projectCenter;
        IGitService _gitService;
        /// <summary>
        /// 0=stop 1=running 2=pause
        /// </summary>
        int _status = 0;
        bool _disposed;
        //记录因为分支是新建，从新克隆的项目
        static ConcurrentDictionary<string, bool> _reCloneFolders = new ConcurrentDictionary<string, bool>();
        public DefaultGitRepositoryCommitWatcher(string localRepositoryPath, string gitHash, int intervalSeconds)
        {
            _localRepositoryPath = localRepositoryPath;
            _gitHash = gitHash;
            _projectCenter = Global.ServiceProvider.GetRequiredService<ProjectCenter>();
            _intervalSeconds = intervalSeconds;
            _logger = Global.ServiceProvider.GetRequiredService<ILogger<IGitRepositoryCommitWatcher>>();
            _gitService = Global.ServiceProvider.GetRequiredService<IGitService>();
        }

        public void Start()
        {
            if (_status != 0)
                return;
            _status = 1;
            runInTask();
        }

        public void Continue()
        {
            _status = 1;
        }

        public async Task PauseAsync()
        {
            //防止多个线程同时调用暂停
            while (true)
            {
                if (_disposed)
                    return;

                if (_status != 1)
                {
                    await Task.Delay(1000);
                    continue;
                }

                if (Interlocked.CompareExchange(ref _status, 2, 1) == 1)
                {
                    return;
                }

                await Task.Delay(1000);
            }
        }


        async void runInTask()
        {
            while (_status != 0 && !_disposed)
            {
                try
                {
                    if (_status == 0)
                        return;

                    if (Directory.Exists(_localRepositoryPath) == false)
                    {
                        FolderMiss?.Invoke(this, null);
                        return;
                    }

                    if (Changed != null)
                    {
                        string[] changeFiles = null;
                        if (_status == 1 && _projectCenter.NeedToWatchChanges(_gitHash) && (changeFiles = await hasChanged()) != null)
                        {
                            if (changeFiles != null && changeFiles.Length > 0)
                            {
                                await Changed.Invoke(this, changeFiles);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "");
                }
                await Task.Delay(_intervalSeconds);
            }
        }

        /// <summary>
        /// 当前分支是否已经被重置
        /// </summary>
        /// <returns></returns>
        public bool IsBranchReset()
        {

            var projectModel = _projectCenter.GetProjectByGitHash(_gitHash);
            if (string.IsNullOrWhiteSpace(projectModel.GitUrl))
                return false;
            return _gitService.IsBranchReset(projectModel.GitRemote, projectModel.BranchName, projectModel.GitUserName, projectModel.GetGitPassword(), _localRepositoryPath);
        }

        async Task<string[]> hasChanged()
        {
            if (_reCloneFolders.ContainsKey(_localRepositoryPath))
            {
                if (_reCloneFolders.TryRemove(_localRepositoryPath, out _))
                {
                    return new string[] { "Git仓库重新克隆" };
                }
            }

            try
            {
                var projectModel = _projectCenter.GetProjectByGitHash(_gitHash);
                var changeRet = await _gitService.CheckHasCommits(projectModel.GitRemote, _localRepositoryPath, projectModel.BranchName, projectModel.GitUserName, projectModel.GetGitPassword());
                if (changeRet.IsBranchReseted)
                {
                    while (true)
                    {
                        try
                        {
                            //分支是新建的
                            SysUtility.DeleteFolder(_localRepositoryPath);
                            _reCloneFolders[_localRepositoryPath] = true;
                            return null;
                        }
                        catch (Exception ex)
                        {

                            try
                            {
                                Error?.Invoke(this, new Exception($"{projectModel.Name}的{projectModel.BranchName}分支是新建的，需要重新克隆，在{_localRepositoryPath}目录删除时发生异常,{ex.Message}"));
                            }
                            catch
                            {

                            }
                            Thread.Sleep(3000);
                        }
                    }
                }

                return changeRet.ChangedFiles;
            }
            catch
            {
                return null;
            }
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}
