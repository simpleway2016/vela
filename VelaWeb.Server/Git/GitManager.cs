using VelaWeb.Server.DBModels;
using VelaWeb.Server.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Way.Lib;
using Org.BouncyCastle.Crypto.Macs;

namespace VelaWeb.Server.Git
{
    public class GitManager
    {
        ConcurrentDictionary<string, GitWorker> _workers = new ConcurrentDictionary<string, GitWorker>();
        private readonly ProjectCenter _projectCenter;

        public GitManager(ProjectCenter projectCenter)
        {
            _projectCenter = projectCenter;
            _projectCenter.ProjectUpdated += _projectCenter_ProjectUpdated;
            _projectCenter.ProjectDeleted += _projectCenter_ProjectDeleted;

        }

        public GitWorker GetWorker(ProjectModel projectModel)
        {
            return _workers[projectModel.GetGitHash()];
        }

        public void RemoveWorker(string githash)
        {
            //如果没有其他工程用这个存储库,那么尝试移除
            if (_workers.TryRemove(githash, out GitWorker worker))
            {
                //再次判断，如果又有人用了这个存储库，那么，重新把worker尝试添加回去，如果添加失败，证明有新的
                if (_projectCenter.GetAllProjectsByGitHash(githash).Length == 0)
                {
                    //确定没人使用，那么释放worker
                    worker.Dispose();
                }
                else
                {
                    if (_workers.TryAdd(githash, worker) == false)
                    {
                        //应该是新建了一个worker，这个可以释放掉
                        worker.Dispose();
                    }
                }
            }
        }

        private async void _projectCenter_ProjectDeleted(object? sender, ProjectModel projectModel)
        {
            var githash = projectModel.GetGitHash();


            if (_projectCenter.GetAllProjectsByGitHash(githash).Length == 0)
            {
                //如果没有其他工程用这个存储库,那么尝试移除
                if (_workers.TryRemove(githash, out GitWorker worker))
                {
                    //再次判断，如果又有人用了这个存储库，那么，重新把worker尝试添加回去，如果添加失败，证明有新的
                    if (_projectCenter.GetAllProjectsByGitHash(githash).Length == 0)
                    {
                        //确定没人使用，那么释放worker
                        worker.Dispose();
                    }
                    else
                    {
                        if (_workers.TryAdd(githash, worker) == false)
                        {
                            //应该是新建了一个worker，这个可以释放掉
                            worker.Dispose();
                        }
                    }
                }
            }
        }

        private void _projectCenter_ProjectUpdated(object? sender, ProjectModel projectModel)
        {
            var githash = projectModel.GetGitHash();
            if (_workers.ContainsKey(githash) == false)
            {
                if (_workers.TryAdd(githash, new GitWorker(githash, projectModel)))
                {
                    _workers[githash].Init();
                }
            }
        }

    }
}
