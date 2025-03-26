using Microsoft.EntityFrameworkCore;
using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using VelaAgent.DBModels;
using VelaAgent.Infrastructures;
using VelaLib;

namespace VelaAgent.Infrastructures.ProjectRunners
{
    public class ProgramRunner : IProjectRunner
    {
        public Project_RunTypeEnum RunnerType => Project_RunTypeEnum.Program;
        public Project Project { get; set; }
        public IInfoOutput InfoOutput { get; set; }

        ILogger<ProgramRunner> _logger;
        ICmdRunner _cmdRunner;
        IProcessService _processService;
        public ProgramRunner(Project project)
        {
            _cmdRunner = Global.ServiceProvider.GetRequiredService<ICmdRunner>();
            _processService = Global.ServiceProvider.GetRequiredService<IProcessService>();
            _logger = Global.ServiceProvider.GetRequiredService<ILogger<ProgramRunner>>();

            Project = project;
        }
        Process _process;
        public bool KeepAlive()
        {
            if (Project == null)
            {

                return false;
            }
            if (_process == null && Project.RunType == Project_RunTypeEnum.Program)
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
                        else
                        {
                            if (!string.IsNullOrEmpty(Project.LogPath))
                            {
                                _ = WriteLog(_process);
                            }
                        }
                    }
                }
                catch
                {

                }
            }

            return _process != null;
        }

        private void _process_Exited(object? sender, EventArgs e)
        {
            if (Project == null)
            {

                return;
            }
            _logger?.LogDebug($"{Project.Name}进程退出, process id:{Project.ProcessId}");

            using var db = new SysDBContext();
            this.Project = db.Project.FirstOrDefault(m => m.Guid == Project.Guid);
            Project.ProcessId = null;
            db.Update(Project);

            if (_process != null && _process.ExitCode != 0)
            {
                _process?.Dispose();
                _process = null;

                if (Project.IsStopped == false)
                {
                    Thread.Sleep(1000);
                    Start();
                }

            }
            else
            {
                _process?.Dispose();
                _process = null;
            }
        }

        FileStream _fs;
        static byte[] EndData = new byte[] { 13, 10 };
        async Task WriteLog(Process process)
        {

            _fs = null;
            try
            {
                string path;
                if (this.Project.LogPath?.StartsWith("/") == true)
                {
                    path = this.Project.LogPath;
                }
                else
                {
                    path = Path.Combine(Global.AppConfig.Current.PublishRootPath, this.Project.Name, this.Project.LogPath);
                }
                var maxSize = Project.LogMaxSize.GetValueOrDefault() * 1024 * 1024;

                _fs = File.Exists(path) ? new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.ReadWrite) : new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                //定位到文件末尾
                _fs.Seek(0, SeekOrigin.End);

                _ = writeToFile(process, process.StandardError, maxSize, path,true);
                await writeToFile(process, process.StandardOutput,  maxSize,path,false);
            }
            catch (Exception)
            {

            }
            finally
            {
                _fs?.Dispose();
                _fs = null;
            }
        }

        async Task writeToFile(Process process,StreamReader streamReader,double maxSize,string path,bool isError)
        {
            try
            {
                while (true)
                {
                    var line = await streamReader.ReadLineAsync();
                    if (line != null)
                    {
                        if (isError)
                            line = $"\x1b[38;5;210m{line}\x1b[0m";

                        lock (this)
                        {
                            _fs.Write(Encoding.UTF8.GetBytes(line));
                            _fs.Write(EndData);
                            _fs.Flush();
                            if (maxSize > 0 && _fs.Length > maxSize)
                            {
                                _fs.Dispose();
                                File.Delete(path);
                                _fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

                            }
                        }
                    }
                    if (process.HasExited)
                    {
                        break;
                    }
                }
            }
            catch
            {

            }
        }


        /// <summary>
        /// 启动程序，并保持运行
        /// </summary>
        public async Task Start()
        {
            using var db = new SysDBContext();
            Project = await db.Project.FirstOrDefaultAsync(m => m.id == Project.id);
            if (Project == null || Project.RunType != Project_RunTypeEnum.Program)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(Project.RunCmd))
                return;

            var publishPath = Path.Combine(Global.AppConfig.Current.PublishRootPath, Project.Name);
            if (!string.IsNullOrWhiteSpace(Project.PublishPath))
            {
                publishPath = Project.PublishPath;
            }
            if (publishPath.StartsWith("."))
            {
                publishPath = Path.GetFullPath(publishPath, AppDomain.CurrentDomain.BaseDirectory);
            }

            _logger?.LogDebug($"准备运行 {publishPath} {Project.RunCmd}");

            if (Project.RunType == Project_RunTypeEnum.Program)
            {

                if (ProjectRunnerHelper.SetProjectStarting(Project.Guid) == false)
                    return;

                try
                {
                    if (string.IsNullOrEmpty(Project.LogPath))
                    {
                        _process = _cmdRunner.RunNoOutput(publishPath, Project.RunCmd);
                    }
                    else
                    {
                        _process = _cmdRunner.Run(publishPath, Project.RunCmd);
                    }
                    _process.WaitForExit(1000);

                    _logger?.LogDebug($"{Project.Name}进程启动, process id:{_process.Id} cmd:{Project.RunCmd}");
                    if (_process.HasExited)
                    {
                        _logger?.LogDebug($"{Project.Name}进程 process id:{_process.Id} 已退出");
                        if (_process.ExitCode != 0)
                        {
                            throw new ServiceException($"{Project.Name}进程意外结束");
                        }
                    }

                    _process.EnableRaisingEvents = true;
                    _process.Exited += _process_Exited;
                    Project.ProcessId = _process.Id;
                    Project.IsStopped = false;
                    await db.UpdateAsync(Project);

                    if (!string.IsNullOrEmpty(Project.LogPath))
                    {
                        _ = WriteLog(_process);
                    }
                }
                finally
                {
                    ProjectRunnerHelper.SetProjectStartCompleted(Project.Guid);
                }

            }
        }

        public async Task Stop()
        {
            if (Project == null)
            {

                return;
            }


            if (_process != null)
            {
                _process.EnableRaisingEvents = false;
                _process.Exited -= _process_Exited;
                _process.Dispose();
                _process = null;
            }

            if (Project != null && Project.ProcessId > 0)
            {
                _logger?.LogDebug($"{Project.Name}进程kill, process id:{Project.ProcessId}");
                _processService.Kill((int)Project.ProcessId.Value);
                using var db = new SysDBContext();
                Project.ProcessId = null;
                await db.UpdateAsync(Project);
            }
        }

        public Task DeleteProject()
        {
            return Task.CompletedTask;
        }
    }
}
