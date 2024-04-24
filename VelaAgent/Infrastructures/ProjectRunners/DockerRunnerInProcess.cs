using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.RegularExpressions;
using VelaAgent.DBModels;
using VelaAgent.Dtos;
using VelaAgent.Infrastructures;
using VelaLib;

namespace VelaAgent.Infrastructures.ProjectRunners
{
    /// <summary>
    /// 由VelaAgent来负责容器崩溃后的自动重启
    /// </summary>
    public class DockerRunnerInProcess : IProjectRunner
    {
        public Project_RunTypeEnum RunnerType => Project_RunTypeEnum.Docker;
        public Project Project { get; set; }
        public IInfoOutput InfoOutput { get; set; }

        ILogger<ProgramRunner> _logger;
        ICmdRunner _cmdRunner;
        IProcessService _processService;
        IDockerEngine _dockerEngine;
        ProjectTtyWorker _ProjectTtyWorker;
        Process _process;

        TtySize _lastTtySize = new TtySize()
        {
            Cols = 120,
            Rows = 44
        };

        public DockerRunnerInProcess(Project project)
        {
            _cmdRunner = Global.ServiceProvider.GetRequiredService<ICmdRunner>();
            _processService = Global.ServiceProvider.GetRequiredService<IProcessService>();
            _logger = Global.ServiceProvider.GetRequiredService<ILogger<ProgramRunner>>();
            _dockerEngine = Global.ServiceProvider.GetRequiredService<IDockerEngine>();
            _ProjectTtyWorker = Global.ServiceProvider.GetRequiredService<ProjectTtyWorker>();
            Project = project;
        }

        public bool KeepAlive()
        {
            if (Project == null)
            {

                return false;
            }
            if (_process == null && Project.RunType == Project_RunTypeEnum.Docker)
            {
                try
                {
                    _process = Process.GetProcessById((int)Project.ProcessId.GetValueOrDefault());
                    if (_process != null)
                    {
                        _process.EnableRaisingEvents = true;
                        _process.Exited += _process_Exited;
                        if (_process.HasExited)
                        {
                            _process.Dispose();
                            _process = null;
                        }
                    }
                }
                catch
                {

                }
            }

            return _process != null;
        }


        /// <summary>
        /// 启动程序，并保持运行
        /// </summary>
        public async Task Start()
        {
            using var db = new SysDBContext();
            Project = await db.Project.FirstOrDefaultAsync(m => m.id == Project.id);
            if (Project == null || Project.RunType != Project_RunTypeEnum.Docker)
            {

                return;
            }
            if (ProjectRunnerHelper.SetProjectStarting(Project.Guid) == false)
                return;

            try
            {
                var publishPath = Path.Combine(Global.AppConfig.Current.PublishRootPath, Project.Name);
                if (!string.IsNullOrWhiteSpace(Project.PublishPath))
                {
                    publishPath = Project.PublishPath;
                }
                if (publishPath.StartsWith("."))
                {
                    publishPath = Path.GetFullPath(publishPath, AppDomain.CurrentDomain.BaseDirectory);
                }

                if (Project.RunType == Project_RunTypeEnum.Docker)
                {
                    var dockerfilepath = Path.Combine("./Dockerfiles", Project.Guid);
                    string dockerfilecontent = File.ReadAllText(dockerfilepath, System.Text.Encoding.UTF8).Replace("%uid%", Program.ProcessUserId.ToString());
                    
                    var targetfilepath = Path.Combine(publishPath, "Dockerfile");
                    //拷贝docker文件
                    //File.Copy(dockerfilepath, targetfilepath, true);
                    File.WriteAllText(targetfilepath, dockerfilecontent, System.Text.Encoding.UTF8);

                    var imageName = $"{Project.Guid}:latest";
                    bool tobuild = false;
                    if (Project.FileHasUpdated)
                    {

                        tobuild = true;
                    }
                    else
                    {
                        var images = await _dockerEngine.GetImages();
                        if (images.Contains(imageName) == false)
                        {
                            tobuild = true;
                        }
                    }

                    var allContainers = await _dockerEngine.GetContainers();
                    var container = allContainers.FirstOrDefault(m => m.ImageName == imageName);

                    if (container != null && container.State == "running")
                    {
                        await _dockerEngine.StopContainer(container.Id);
                    }

                    if (tobuild)
                    {
                        //编译映像
                        if (InfoOutput != null)
                        {
                            await InfoOutput.Output("正在编译Docker映像...");
                        }

                        if (container != null)
                        {
                            await _dockerEngine.RemoveContainer(container.Id);
                        }
                        container = null;

                        var images = await _dockerEngine.GetImages();
                        if (images.Contains(imageName))
                        {
                            await _dockerEngine.RemoveImage(imageName);
                            //await _dockerEngine.PruneImages();
                        }

                        var worker = Global.ServiceProvider.GetRequiredService<TtyWorker>();
                        try
                        {
                            _ProjectTtyWorker[Project.Guid] = worker;
                            if (InfoOutput != null && InfoOutput.Cols > 0 && InfoOutput.Rows > 0)
                            {
                                await worker.Init(Project.Guid, InfoOutput.Cols, InfoOutput.Rows);
                                _lastTtySize.Cols = InfoOutput.Cols;
                                _lastTtySize.Rows = InfoOutput.Rows;
                            }
                            else
                            {
                                await worker.Init(Project.Guid, _lastTtySize.Cols, _lastTtySize.Rows);
                            }

                            if (OperatingSystem.IsWindows())
                            {
                                worker.SendCommand($"cd /d \"{publishPath}\"");
                            }
                            else
                            {
                                worker.SendCommand($"cd \"{publishPath}\"");
                            }

                            worker.Received += (s, data) =>
                            {
                                if (InfoOutput != null)
                                    return InfoOutput.Output(data);

                                return Task.CompletedTask;
                            };
                            await worker.SendCommands(new string[] { $"docker build -t {imageName} ." });

                        }
                        finally
                        {
                            _ProjectTtyWorker.TryRemove(Project.Guid, out _);
                            worker?.Dispose();
                        }
                        //await _dockerEngine.Build(InfoOutput, publishPath, imageName , (text) => { 
                        //    this.InfoOutput?.Output(text);
                        //});

                        if (Project.FileHasUpdated)
                        {
                            Project.FileHasUpdated = false;
                            await db.UpdateAsync(Project);
                        }
                    }



                    if (container != null)
                    {
                        if (InfoOutput != null)
                        {
                            await InfoOutput.Output("正在启动容器...");
                        }

                        if (container.State != "running")
                        {
                            _process = _dockerEngine.StartContainerInProcess(container.Id);
                        }
                    }
                    else
                    {
                        if (InfoOutput != null)
                        {
                            await InfoOutput.Output("正在创建并启动容器...");
                        }

                        string[] portMaps = null;
                        if (!string.IsNullOrWhiteSpace(Project.DockerPortMap))
                        {
                            portMaps = Regex.Split(Project.DockerPortMap, ",|，");
                        }

                        string[] folderMaps = null;
                        if (!string.IsNullOrWhiteSpace(Project.DockerFolderMap))
                        {
                            folderMaps = Regex.Split(Project.DockerFolderMap, ",|，");
                        }

                        string[] envMaps = null;
                        if (!string.IsNullOrWhiteSpace(Project.DockerEnvMap))
                        {
                            envMaps = Regex.Split(Project.DockerEnvMap, ",|，");
                        }
                        try
                        {
                            await _dockerEngine.CreateImage(imageName, $"{Project.Name}_{Project.Guid}", publishPath, Project.IsHostNetwork, portMaps, folderMaps, envMaps, Project.MemoryLimit);
                            allContainers = await _dockerEngine.GetContainers();
                            container = allContainers.FirstOrDefault(m => m.ImageName == imageName);
                            if (container == null)
                                throw new Exception("创建容器失败");
                            if (container.State != "running")
                            {
                                _process = _dockerEngine.StartContainerInProcess(container.Id);
                            }
                        }
                        catch (Exception ex)
                        {

                            try
                            {
                                //执行docker失败，移除docker container
                                allContainers = await _dockerEngine.GetContainers();
                                container = allContainers.FirstOrDefault(m => m.ImageName == imageName);
                                if (container != null)
                                {
                                    await _dockerEngine.RemoveContainer(container.Id);
                                }
                            }
                            catch
                            {
                            }

                            throw ex;
                        }
                        allContainers = await _dockerEngine.GetContainers();
                        container = allContainers.FirstOrDefault(m => m.ImageName == imageName);
                    }

                    if (container != null)
                    {
                        _process.EnableRaisingEvents = true;
                        _process.Exited += _process_Exited;
                        if (_process.HasExited == false)
                        {
                            Project.ProcessId = _process.Id;
                            Project.DockerContainerId = container.Id;
                            Project.IsStopped = false;
                            await db.UpdateAsync(Project);
                        }
                    }
                }
            }
            finally
            {
                ProjectRunnerHelper.SetProjectStartCompleted(this.Project.Guid);
            }

        }

        private void _process_Exited(object? sender, EventArgs e)
        {
            if (Project == null)
            {

                return;
            }
            _logger?.LogDebug($"{Project.Name}进程退出, process id:{Project.ProcessId}");
         
            using var db = new SysDBContext();
            Project.ProcessId = null;
            db.Update(Project);

        

            if (_process != null && _process.ExitCode != 0)
            {
                _process?.Dispose();
                _process = null;

                Thread.Sleep(1000);
                Start();
            }
            else
            {
                _process?.Dispose();
                _process = null;
            }
        }

        public async Task Stop()
        {
            if (_process != null)
            {
                _process.EnableRaisingEvents = false;
                _process.Exited -= _process_Exited;
                _process.Dispose();
                _process = null;
            }

            var imageName = $"{Project.Guid}:latest";
            var allContainers = await _dockerEngine.GetContainers();
            var container = allContainers.FirstOrDefault(m => m.ImageName == imageName);
            if (container != null)
            {
                await _dockerEngine.StopContainer(container.Id);
            }

            using var db = new SysDBContext();
            Project.ProcessId = null;
            await db.UpdateAsync(Project);
        }

        public async Task DeleteProject()
        {
            var imageName = $"{Project.Guid}:latest";
            _dockerEngine.RemoveContainer(Project.DockerContainerId);
            _dockerEngine.RemoveImage(imageName);
        }
    }
}
