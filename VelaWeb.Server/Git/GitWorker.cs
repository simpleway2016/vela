using Microsoft.Extensions.DependencyInjection;
using VelaLib;
using VelaWeb.Server.Infrastructures;
using VelaWeb.Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VelaWeb.Server.Git
{
    public class GitWorker : IDisposable
    {
        bool _disposed;
        string _gitFolder;
        IGitService _gitService;
        ProjectCenter _projectCenter;
        private readonly string _gitHash;
        IGitRepositoryCommitWatcher _gitRepositoryCommitWatcher;
        public bool Ready => _gitRepositoryCommitWatcher != null;
        bool _cloneCompleted = false;

        public bool Disposed => _disposed;
        public string GitUrl { get; private set; }
        public GitWorker(string gitHash, ProjectModel projectModel)
        {
            _gitHash = gitHash;
            _gitService = Global.ServiceProvider.GetRequiredService<IGitService>();
            _projectCenter = Global.ServiceProvider.GetRequiredService<ProjectCenter>();
            GitUrl = projectModel.GitUrl;
            _gitFolder = $"./ProjectCodes/{_gitHash}";

        }

        public void Init()
        {
            if (string.IsNullOrWhiteSpace(GitUrl) == false && File.Exists($"{_gitFolder}/.git/index_completed") == false)
            {
                if (Directory.Exists(_gitFolder))
                {
                    try
                    {
                        SysUtility.DeleteFolder(_gitFolder);
                    }
                    catch (Exception ex)
                    {
                        _projectCenter.OnGitBeginClone($"删除文件夹{_gitFolder}失败，{ex.Message}\r\n30秒后自动重试");
                        Task.Run(async () =>
                        {
                            await Task.Delay(30000);
                            Init();
                        });
                        return;
                    }
                }
                Directory.CreateDirectory(_gitFolder);
                clone();
            }
            else
            {
                if (Directory.Exists(_gitFolder) == false)
                {
                    Directory.CreateDirectory(_gitFolder);
                }
                _cloneCompleted = true;
                //监控存储库的变化
                watchChanges();
            }
        }

        /// <summary>
        /// 重新克隆
        /// </summary>
        public void ReClone()
        {
            if (Directory.Exists(_gitFolder))
            {
                try
                {
                    SysUtility.DeleteFolder(_gitFolder);
                }
                catch (Exception ex)
                {
                    throw new ServiceException($"删除文件夹{_gitFolder}失败，{ex.Message}\r\n请手动删除此文件夹");
                }
            }
        }

        void watchChanges()
        {
            _gitRepositoryCommitWatcher = new DefaultGitRepositoryCommitWatcher(_gitFolder, _gitHash, 3000);
            if (!string.IsNullOrWhiteSpace(GitUrl))
            {
                _gitRepositoryCommitWatcher.Changed += _gitRepositoryCommitWatcher_Changed;
                _gitRepositoryCommitWatcher.Error += _gitRepositoryCommitWatcher_Error;
                _gitRepositoryCommitWatcher.FolderMiss += _gitRepositoryCommitWatcher_FolderMiss;
                _gitRepositoryCommitWatcher.Start();
            }
            else
            {
                _gitRepositoryCommitWatcher.Continue();
            }
        }

        private void _gitRepositoryCommitWatcher_Error(object? sender, Exception e)
        {
            _projectCenter.OnGitCloneError(_gitHash, e.Message);
        }

        private void _gitRepositoryCommitWatcher_FolderMiss(object? sender, EventArgs e)
        {
            //git文件夹消失了，需要重新克隆
            _gitRepositoryCommitWatcher.Dispose();
            _gitRepositoryCommitWatcher = null;
            Init();
        }

        public Task Pause()
        {
            if (!_cloneCompleted)
                return Task.CompletedTask;

            if (_gitRepositoryCommitWatcher == null)
                throw new ServiceException("存储库尚未克隆完毕");

            return _gitRepositoryCommitWatcher.PauseAsync();
        }

        public bool IsBranchReset()
        {
            if (!_cloneCompleted)
                throw new ServiceException("存储库尚未克隆完毕");

            if (_gitRepositoryCommitWatcher == null)
                throw new ServiceException("存储库尚未克隆完毕");

            return _gitRepositoryCommitWatcher.IsBranchReset();
        }

        public void Continue()
        {
            if (_gitRepositoryCommitWatcher == null)
                throw new ServiceException("存储库尚未克隆完毕");

            _gitRepositoryCommitWatcher.Continue();
        }

        async void clone()
        {
            if (_disposed)
                return;

            try
            {
                _projectCenter.OnGitBeginClone(_gitHash);
                //找一个projectModel用它的用户信息
                var authModel = _projectCenter.GetProjectByGitHash(_gitHash);

                var outputId = DateTime.Now.Ticks;
                DateTime startTime = DateTime.Now;
                await _gitService.CloneAsync(authModel.GitUrl, authModel.GitUserName, authModel.GetGitPassword(),
                        authModel.BranchName,
                        _gitFolder, (url, received, total, indexed) =>
                        {
                            if ((DateTime.Now - startTime).TotalSeconds >= 2)
                            {
                                startTime = DateTime.Now;
                                _projectCenter.OnGitCloning(url, outputId, _gitHash, received, total, indexed);
                            }

                            return !_disposed;
                        });

                if (_disposed)
                    return;

                File.WriteAllBytes($"{_gitFolder}/.git/index_completed", new byte[] { 0x1 });

                _projectCenter.OnGitCloneCompleted(_gitHash);
                _cloneCompleted = true;

                //监控存储库的变化
                watchChanges();
            }
            catch (Exception ex)
            {
                if (_disposed)
                    return;

                while (ex.InnerException != null)
                    ex = ex.InnerException;

                _projectCenter.OnGitCloneError(_gitHash, ex.Message);

                await Task.Delay(30000);

                if (Directory.Exists(_gitFolder))
                {
                    while (true)
                    {
                        try
                        {
                            SysUtility.DeleteFolder(_gitFolder);
                            break;
                        }
                        catch (Exception ex2)
                        {
                            _projectCenter.OnGitCloneError(_gitHash, $"删除文件夹{_gitFolder}失败，{ex2.Message}\r\n30秒后重试");

                            await Task.Delay(30000);
                            continue;
                        }
                    }
                }
                Task.Run(clone);
            }
        }

        private Task _gitRepositoryCommitWatcher_Changed(object? sender, string[] changeFiles)
        {
            return _projectCenter.OnGitChanged(this, _gitHash, changeFiles);
        }


        public void Dispose()
        {
            _disposed = true;
            _gitRepositoryCommitWatcher?.Dispose();
            _gitRepositoryCommitWatcher = null;
        }
    }
}
