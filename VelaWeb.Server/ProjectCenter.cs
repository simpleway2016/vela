using VelaWeb.Server.DBModels;
using VelaWeb.Server.Models;
using System.Collections.Concurrent;
using System.Net.Http;
using Way.Lib;
using VelaWeb.Server.Infrastructures;
using System.Diagnostics;
using System;
using VelaLib;
using VelaWeb.Server.Git;

namespace VelaWeb.Server
{
    public class ProjectCenter
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProjectCenter> _logger;
        private readonly BuildingManager _buildingManager;
        private readonly IGitService _gitService;
        ConcurrentDictionary<string, ProjectModel> _projectDict = new ConcurrentDictionary<string, ProjectModel>();
        ConcurrentDictionary<string, DateTime> _projectStartTime = new ConcurrentDictionary<string, DateTime>();

        public event EventHandler<ProjectModel> ProjectUpdated;
        public event EventHandler<ProjectModel> ProjectDeleted;

        IProjectBuildInfoOutput _buildInfoOutput;
        IProjectBuildInfoOutput ProjectBuildInfoOutput
        {
            get
            {
                return _buildInfoOutput ??= Global.ServiceProvider.GetRequiredService<IProjectBuildInfoOutput>();
            }
        }

        public ProjectCenter(IHttpClientFactory httpClientFactory, ILogger<ProjectCenter> logger,
            BuildingManager buildingManager,
            IGitService gitService)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _buildingManager = buildingManager;
            _gitService = gitService;
        }
        public void Init()
        {
            //确保gitManager已被实例化，因为ProjectUpdated事件主要传递给它
            Global.ServiceProvider.GetRequiredService<GitManager>();

            using var db = new SysDBContext();
            var agents = db.Agent.ToArray();

            foreach (var item in agents)
            {
                new Thread(() => { loadServiceListByServer(item.ToJsonString().FromJson<AgentModel>()); }).Start();
            }

        }

        public DateTime GetProjectStartTime(string guid)
        {
            if (_projectStartTime.TryGetValue(guid, out DateTime dateTime))
            {
                return dateTime;
            }
            else
            {
                _projectStartTime[guid] = DateTime.Now.AddMinutes(-1);
                return _projectStartTime[guid];
            }
        }

        public void SetProjectStartTime(string guid, DateTime dateTime)
        {
            _projectStartTime[guid] = dateTime;
        }

        /// <summary>
        /// 强制停止编译
        /// </summary>
        /// <param name="guid"></param>
        public async Task StopBuild(string guid)
        {
            var workers = _buildingManager.GetWorkers(guid);
            if (workers != null)
            {
                foreach (var worker in workers)
                {
                    await worker.Kill();
                }
            }
        }


        async void loadServiceListByServer(AgentModel agentModel)
        {
            while (true)
            {
                var client = _httpClientFactory.CreateClient("");
                try
                {
                    var ret = await client.GetStringAsync($"https://{agentModel.Address}:{agentModel.Port}/Publish/GetAllProjects");
                    var projects = ret.FromJson<ProjectModel[]>();
                    foreach (var projectModel in projects)
                    {
                        //假设启动时间是1分钟前
                        SetProjectStartTime(projectModel.Guid, DateTime.Now.AddMinutes(-1));
                        projectModel.OwnerServer = agentModel;
                        projectModel.ProcessId = null;
                        projectModel.DockerContainerId = null;
                        _projectDict[projectModel.Guid] = projectModel;
                        try
                        {
                            ProjectUpdated?.Invoke(this, projectModel);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "");
                        }
                    }
                    break;
                }
                catch (Exception ex)
                {
                    await Task.Delay(2000);
                }
            }
        }
        public void OnNewAgent(Agent agent)
        {
            new Thread(() => { loadServiceListByServer(agent.ToJsonString().FromJson<AgentModel>()); }).Start();
        }
        public void OnRemoveAgent(long agentId)
        {
            foreach (var pair in _projectDict)
            {
                if (pair.Value.OwnerServer.id == agentId)
                {
                    _projectDict.TryRemove(pair.Key, out _);
                }
            }
        }

        public ProjectModel[] GetAllProjects()
        {
            return _projectDict.Select(m => m.Value).ToArray();
        }

        public ProjectModel GetProject(string guid)
        {
            return _projectDict.Where(m => m.Value.Guid == guid).Select(m => m.Value).FirstOrDefault();
        }

        public ProjectModel[] GetAllProjectsByGitHash(string gitHash)
        {
            return _projectDict.Where(m => m.Value.GetGitHash() == gitHash).Select(m => m.Value).ToArray();
        }

        /// <summary>
        /// 查询指定git是否有自动发布的程序
        /// </summary>
        /// <param name="gitHash"></param>
        /// <returns></returns>
        public bool NeedToWatchChanges(string gitHash)
        {
            return _projectDict.Any(m => m.Value.PublishMode == 1 && m.Value.GetGitHash() == gitHash);
        }

        public ProjectModel? GetProjectByGitHash(string gitHash)
        {
            return _projectDict.Where(m => m.Value.GetGitHash() == gitHash).Select(m => m.Value).FirstOrDefault();
        }

        public void OnAgentUpdate(Agent agent)
        {
            foreach (var pair in _projectDict)
            {
                if (pair.Value.OwnerServer != null && pair.Value.OwnerServer.id == agent.id)
                {
                    pair.Value.OwnerServer.Category = agent.Category;
                    pair.Value.OwnerServer.Desc = agent.Desc;
                }
            }
        }

        public void OnProjectUpdate(ProjectModel projectModel)
        {
            _projectDict[projectModel.Guid] = projectModel;
            ProjectUpdated?.Invoke(this, projectModel);
        }

        /// <summary>
        /// 刷新project model的信息
        /// </summary>
        /// <param name="projectModel"></param>
        public void RefreshProject(ProjectModel projectModel)
        {
            if (projectModel.OwnerServer != null)
            {
                loadServiceListByServer(projectModel.OwnerServer);
            }
        }

        public void OnProjectDelete(ProjectModel projectModel)
        {
            if (_projectDict.TryRemove(projectModel.Guid, out ProjectModel o))
            {
                ProjectDeleted?.Invoke(this, projectModel);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gitHash"></param>
        public async void OnGitBeginClone(string gitHash)
        {
            var projectModels = GetAllProjectsByGitHash(gitHash);
            foreach (var projectModel in projectModels)
            {
                projectModel.Status = "正在克隆存储库...";
                projectModel.Error = null;
                await ProjectBuildInfoOutput.OutputBuildInfoAsync(projectModel, "正在克隆存储库...", false);
            }
        }

        public async void OnGitError(string gitHash, string error)
        {
            var projectModels = GetAllProjectsByGitHash(gitHash);
            foreach (var projectModel in projectModels)
            {
                projectModel.Status = "Git发生异常";
                projectModel.Error = "Git发生异常";
                await ProjectBuildInfoOutput.OutputBuildInfoAsync(projectModel, error, false);
            }
        }

        public async void OnGitCloning(string url, long outputId, string gitHash, int received, int total, int indexed)
        {
            var projectModels = GetAllProjectsByGitHash(gitHash);
            foreach (var projectModel in projectModels)
            {
                projectModel.Status = $"正在克隆存储库{received * 100 / total}%...\r\n{received}/{total} 索引:{indexed}";
                await ProjectBuildInfoOutput.OutputBuildInfoAsync(projectModel, $"\x1b[2K\r正在克隆存储库{Path.GetFileNameWithoutExtension(url)} {received * 100 / total}% {received}/{total} 索引:{indexed}", false);
            }
        }

        public async void OnGitCloneCompleted(string gitHash)
        {
            var projectModels = GetAllProjectsByGitHash(gitHash);
            foreach (var projectModel in projectModels)
            {
                projectModel.Status = $"克隆完毕";
                await ProjectBuildInfoOutput.OutputBuildInfoAsync(projectModel, "\r\n克隆完毕", false);
            }
        }
        public async void OnGitCloneError(string gitHash, string error)
        {
            var projectModels = GetAllProjectsByGitHash(gitHash);
            foreach (var projectModel in projectModels)
            {
                projectModel.Error = error;
                projectModel.Status = $"克隆发生异常";
                await ProjectBuildInfoOutput.OutputBuildInfoAsync(projectModel, error + "\r\n30秒后重试", false);
            }
        }

        /// <summary>
        /// git存储库有更新
        /// </summary>
        /// <param name="gitHash"></param>
        /// <param name="changeFiles"></param>
        public async Task OnGitChanged(GitWorker gitWorker, string gitHash, string[] changeFiles)
        {
            await PullAndBuildAsync(gitWorker, 0, 0, null, gitHash, true, changeFiles);

        }

        /// <summary>
        /// 拉取最新代码
        /// </summary>
        /// <param name="runGuid">运行哪个项目,null表示所有相关项目</param>
        /// <param name="changeFiles">isAuto=true时，这个变量表示哪些文件变化引起的编译</param>
        /// <returns></returns>
        public async Task PullAndBuildAsync(GitWorker gitWorker, int cols, int rows, string runGuid, string gitHash, bool isAuto, string[] changeFiles)
        {
            var gitFolder = $"./ProjectCodes/{gitHash}";

            var projectModels = GetAllProjectsByGitHash(gitHash);
            if (runGuid != null)
            {
                //锁定指定的projectModel
                projectModels = projectModels.Where(m => m.Guid == runGuid).ToArray();
            }

            foreach (var projectModel in projectModels)
            {
                projectModel.Status = "";
                projectModel.Error = null;
                if (changeFiles == null)
                {
                    await ProjectBuildInfoOutput.OutputBuildInfoAsync(projectModel, "准备拉取", true);
                }
                else
                {
                    await ProjectBuildInfoOutput.OutputBuildInfoAsync(projectModel, $"发生变化的文件：{string.Join("\r\n", changeFiles)}", true);
                }
            }

            bool needReclone = false;

            try
            {
                string comment = null;
                if (!string.IsNullOrEmpty(runGuid))
                {
                    comment = $"{projectModels.Where(m => m.Guid == runGuid).FirstOrDefault()?.Name} {gitWorker.GitUrl}";
                }
                else
                    comment = gitWorker.GitUrl;

                string projectName;
                if (!string.IsNullOrEmpty(runGuid))
                {
                    projectName = projectModels.Where(m => m.Guid == runGuid).FirstOrDefault()?.Name;
                }
                else
                {
                    projectName = projectModels.FirstOrDefault()?.Name;
                }
                if (string.IsNullOrEmpty(projectName))
                    projectName = gitWorker.GitUrl;

                using RequestBuilding requestBuilding = new RequestBuilding(_buildingManager, projectName, comment);
                DateTime startTime = DateTime.Now;
                while (true)
                {
                    if (_buildingManager.TryAddRequest(requestBuilding))
                    {
                        break;

                    }
                    else
                    {
                        var requestingComments = _buildingManager.GetRequestingComments();
                        foreach (var projectModel in projectModels)
                        {
                            projectModel.Status = "等候处理";
                            projectModel.Error = null;
                            await ProjectBuildInfoOutput.OutputBuildInfoAsync(projectModel, $"\u001b[2K\r等候 {requestingComments} 结束..{(int)(DateTime.Now - startTime).TotalSeconds}秒", false);
                        }
                        await Task.Delay(1000);
                    }
                }

                //foreach (var projectModel in projectModels)
                //{
                //    projectModel.Status = "申请暂停Git探测";
                //    projectModel.Error = null;
                //    await ProjectBuildInfoOutput.OutputBuildInfoAsync(projectModel, "申请暂停Git探测", false);
                //}

                //有新的提交
                await gitWorker.Pause();

                //foreach (var projectModel in projectModels)
                //{
                //    projectModel.Status = "已暂停Git探测...";
                //    projectModel.Error = null;
                //    await ProjectBuildInfoOutput.OutputBuildInfoAsync(projectModel, "已暂停Git探测", false);
                //}

                var outputId = DateTime.Now.Ticks;

                if (changeFiles != null)
                {
                    List<ProjectModel> findResult = new List<ProjectModel>();
                    if (changeFiles.Length == 1 && changeFiles[0] == "Git仓库重新克隆")
                    {
                        foreach (var projectModel in projectModels)
                        {
                            projectModel.Status = "Git仓库已重新克隆";
                            projectModel.Error = null;
                            await ProjectBuildInfoOutput.OutputBuildInfoAsync(projectModel, "Git仓库已重新克隆", false);
                        }

                        findResult.AddRange(projectModels);
                    }
                    else
                    {
                        foreach (var project in projectModels)
                        {
                            //计算哪些project发生了变化
                            var gitFullPath = Path.GetFullPath(gitFolder, AppDomain.CurrentDomain.BaseDirectory);
                            if( string.IsNullOrWhiteSpace(project.ProgramPath) || project.ProgramPath == "." ||
                                project.ProgramPath == "./" || project.ProgramPath == ".\\" || project.ProgramPath == "/" || project.ProgramPath == "\\")
                            {
                                project.ProgramPath = "";
                                findResult.Add(project);
                                continue;
                            }
                            var programFullPath = Path.GetFullPath(Path.Combine(gitFolder, project.ProgramPath), AppDomain.CurrentDomain.BaseDirectory);
                            foreach (var changeFile in changeFiles)
                            {
                                var changeFullPath = Path.GetFullPath(Path.Combine(gitFolder, changeFile), AppDomain.CurrentDomain.BaseDirectory);
                                var relative = Path.GetRelativePath(programFullPath, changeFullPath);
                                if (relative.StartsWith("..") == false)
                                {
                                    //变化文件属于程序文件夹
                                    findResult.Add(project);
                                    break;
                                }
                            }
                        }
                    }


                    if (findResult.Count == 0 && projectModels.Length > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(projectModels[0].GitUrl))
                        {
                            //虽然没有任何项目与之关联，但这里也要拉取一下，否则，总会检查到有更新，不断输出“准备拉取...”
                            await _gitService.PullAsync(gitFolder, projectModels[0].GitRemote, projectModels[0].GitUserName,
                            projectModels[0].GetGitPassword(), projectModels[0].BranchName, (received, total, indexed) =>
                            {
                                foreach (var projectModel in projectModels)
                                {
                                    projectModel.Status = $"正在拉取分支{projectModel.BranchName}\r\n{received * 100 / total}%...\r\n{received}/{total} 索引:{indexed}";

                                    ProjectBuildInfoOutput.OutputBuildInfoAsync(projectModel, $"\x1b[2K\r正在拉取分支{projectModel.BranchName} {received * 100 / total}% {received}/{total} 索引:{indexed}", false);
                                }

                                return !gitWorker.Disposed;
                            });
                            foreach (var projectModel in projectModels)
                            {
                                projectModel.Status = "拉取完毕";
                                await ProjectBuildInfoOutput.OutputBuildInfoAsync(projectModel, "拉取完毕", false);
                            }
                        }
                    }


                    projectModels = findResult.ToArray();
                }
                else
                {
                    if (gitWorker.IsBranchReset())
                    {
                        //分支已经被重置
                        needReclone = true;
                        throw new Exception("分支已被重置，请等候重新克隆完成后，再尝试");
                    }
                }

                if (projectModels.Length == 0)
                {
                    return;
                }


                if (!string.IsNullOrWhiteSpace(projectModels[0].GitUrl))
                {
                    //移除所有修改
                    foreach (var projectModel in projectModels)
                    {
                        projectModel.Status = "正在检查仓库...";
                        projectModel.Error = null;
                        await ProjectBuildInfoOutput.OutputBuildInfoAsync(projectModel, "正在检查仓库", false);
                    }
                    _gitService.CancelModify(gitFolder);

                    foreach (var projectModel in projectModels)
                    {
                        projectModel.Status = "正在拉取最新代码...";
                        projectModel.Error = null;
                        await ProjectBuildInfoOutput.OutputBuildInfoAsync(projectModel, "正在拉取最新代码", false);
                    }

                    outputId = DateTime.Now.Ticks;
                    await _gitService.PullAsync(gitFolder, projectModels[0].GitRemote, projectModels[0].GitUserName,
                     projectModels[0].GetGitPassword(), projectModels[0].BranchName, (received, total, indexed) =>
                     {
                         foreach (var projectModel in projectModels)
                         {
                             projectModel.Status = $"正在拉取分支{projectModel.BranchName}\r\n{received * 100 / total}%...\r\n{received}/{total} 索引:{indexed}";

                             ProjectBuildInfoOutput.OutputBuildInfoAsync(projectModel, $"\x1b[2K\r正在拉取分支{projectModel.BranchName} {received * 100 / total}% {received}/{total} 索引:{indexed}", false);
                         }
                         return !gitWorker.Disposed;
                     });
                }

                for(int j = 0; j <  projectModels.Length; j ++)
                {
                    var projectModel = projectModels[j];
                    //如果不是最后一个model，requestBuilding不用作为参数传下去，因为传下去会被dispose掉
                    var buildingInfo = j == projectModels.Length - 1 ? requestBuilding : null;

                    requestBuilding.ProjectName = projectModel.Name;

                    projectModel.Status = "成功完成拉取";
                    await ProjectBuildInfoOutput.OutputBuildInfoAsync(projectModel, "成功完成拉取", false);

                    if (projectModel.PublishMode == 1 && isAuto)
                    {
                        await projectModel.BuildAndPublish(buildingInfo, cols, rows);
                    }
                    else if (isAuto == false)
                    {
                        //这是手动启动
                        await projectModel.BuildAndPublish(buildingInfo, cols, rows);
                    }
                    else
                    {
                        await ProjectBuildInfoOutput.OutputBuildInfoAsync(projectModel, "当前为手动启动模式，需要手动点击【立刻发布】", false);
                    }

                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;

                foreach (var projectModel in projectModels)
                {
                    projectModel.Status = null;
                    projectModel.Error = "拉取出错";
                    await ProjectBuildInfoOutput.OutputBuildInfoAsync(projectModel, "拉取出错，" + ex.Message, false);
                }

            }
            finally
            {
                if (needReclone)
                {
                    gitWorker.ReClone();
                }
                gitWorker.Continue();
            }
        }
    }
}
