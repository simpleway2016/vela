using Docker.DotNet.Models;
using Org.BouncyCastle.Ocsp;
using System.Diagnostics;
using System.Text;
using VelaAgent.Dtos;
using VelaLib;
using Way.Lib;

namespace VelaAgent.Infrastructures
{
    public class CmdDockerEngine : IDockerEngine
    {
        readonly ICmdRunner _cmdRunner;
        public CmdDockerEngine(ICmdRunner cmdRunner)
        {
            this._cmdRunner = cmdRunner;

        }
        public async Task Build(IInfoOutput infoOutput, string projectPath, string imageName, Action<string> outputInfoHandler)
        {
            using var process = _cmdRunner.Run(projectPath, $"docker build -t {imageName} .");
            ReadOutputInfo(outputInfoHandler,process);
            ReadErrorInfo(outputInfoHandler, process);
            await process.WaitForExitAsync();
        }

        async void ReadOutputInfo(Action<string> outputInfoHandler, Process process)
        {
            while (true)
            {
                var line = await process.StandardOutput.ReadLineAsync();
                if (line == null)
                    break;

                if (string.IsNullOrWhiteSpace(line) == false)
                {
                    outputInfoHandler(line);
                }
            }
        }


        async void ReadErrorInfo(Action<string> outputInfoHandler, Process process)
        {
            while (true)
            {
                var line = await process.StandardError.ReadLineAsync();
                if (line == null)
                    break;

                if (string.IsNullOrWhiteSpace(line) == false)
                {
                    outputInfoHandler(line);
                }
            }
        }

        public async Task<string[]> GetImages()
        {
            var ret = await _cmdRunner.RunForResult(null, "docker images --format \"{{.Repository}}:{{.Tag}}\"");
            return ret.Split('\n').Select(m => m.Trim()).Where(m => m.Length > 0).ToArray();
        }

        public async Task<DockerContainer[]> GetContainers()
        {
            var ret = await _cmdRunner.RunForResult(null, "docker ps -a --format \"{{.ID}}\\t{{.State}}\\t{{.Image}}\"");
            var lines = ret.Split('\n').Select(m => m.Trim()).Where(m => m.Length > 0).ToArray();
            var containers = new DockerContainer[lines.Length];
            for(int i = 0; i < containers.Length; i ++)
            {
                var item = containers[i] = new DockerContainer();
                var arr = lines[i].Split('\t');
                item.Id = arr[0];
                item.State = arr[1];
                item.ImageName = arr[2];

            }
            return containers;
        }

        public async Task<DockerContainerState[]> GetRunningContainerStates()
        {
            var ret = await _cmdRunner.RunForResult(null, "docker stats --no-stream --format \"{{.ID}}\\t{{.CPUPerc}}\\t{{.MemPerc}}\"");
            var lines = ret.Split('\n').Select(m => m.Trim()).Where(m => m.Length > 0).ToArray();
            var containers = new DockerContainerState[lines.Length];
            for (int i = 0; i < containers.Length; i++)
            {
                var item = containers[i] = new DockerContainerState();
                var arr = lines[i].Split('\t');
                item.Id = arr[0];
                item.State = "running";
                try
                {
                    item.CpuPerc = double.Parse(arr[1].Replace("%", ""));
                }
                catch
                {
                }
                try
                {
                    item.MemPerc = double.Parse(arr[2].Replace("%", ""));
                }
                catch
                {
                }

            }
            return containers;
        }

       
        public async Task RunImage(string imageName, string containerName, string programPath, bool hostNetwork, IEnumerable<string> portMaps, IEnumerable<string> folderMaps, IEnumerable<string> envMaps, string memoryLimit)
        {
            StringBuilder cmdBuilder = new StringBuilder();
            cmdBuilder.Append($"docker run -d --name={containerName} -v \"{programPath}\":/vela/app --restart=unless-stopped");//on-failure:3
            if (hostNetwork == false && portMaps != null)
            {
                foreach( var p in portMaps)
                {
                    if (!string.IsNullOrWhiteSpace(p))
                    {
                        cmdBuilder.Append($" -p {p.Trim()}");
                    }
                }
            }
            else if (hostNetwork)
            {
                cmdBuilder.Append($" --network=host");
            }

            if (folderMaps!= null)
            {
                foreach (var v in folderMaps)
                {
                    if (!string.IsNullOrWhiteSpace(v))
                    {
                      if (v.Trim().StartsWith("\"") == false)
                        {
                            cmdBuilder.Append($" -v \"{v.Trim()}\"");
                        }
                        else
                        {
                            cmdBuilder.Append($" -v {v.Trim()}");
                        }
                    }
                }
            }

            if (envMaps != null)
            {
                foreach (var v in envMaps)
                {
                    if (!string.IsNullOrWhiteSpace(v))
                    {
                        if (v.Trim().StartsWith("\"") == false)
                        {
                            cmdBuilder.Append($" -e \"{v.Trim()}\"");
                        }
                        else
                        {
                            cmdBuilder.Append($" -e {v.Trim()}");
                        }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(memoryLimit))
            {
                cmdBuilder.Append($" -m {memoryLimit}");
            }
            cmdBuilder.Append(' ');
            cmdBuilder.Append(imageName);
            await _cmdRunner.RunForResult(null, cmdBuilder.ToString());
            cmdBuilder.Clear();
        }

        public async Task CreateImage(string imageName, string containerName, string programPath, bool hostNetwork, IEnumerable<string> portMaps, IEnumerable<string> folderMaps, IEnumerable<string> envMaps, string memoryLimit)
        {
            StringBuilder cmdBuilder = new StringBuilder();
            cmdBuilder.Append($"docker create --name={containerName} -v \"{programPath}\":/vela/app");
            if (hostNetwork == false && portMaps != null)
            {
                foreach (var p in portMaps)
                {
                    if (!string.IsNullOrWhiteSpace(p))
                    {
                        cmdBuilder.Append($" -p {p.Trim()}");
                    }
                }
            }
            else if (hostNetwork)
            {
                cmdBuilder.Append($" --network=host");
            }

            if (folderMaps != null)
            {
                foreach (var v in folderMaps)
                {
                    if (!string.IsNullOrWhiteSpace(v))
                    {
                        if (v.Trim().StartsWith("\"") == false)
                        {
                            cmdBuilder.Append($" -v \"{v.Trim()}\"");
                        }
                        else
                        {
                            cmdBuilder.Append($" -v {v.Trim()}");
                        }
                    }
                }
            }

            if (envMaps != null)
            {
                foreach (var v in envMaps)
                {
                    if (!string.IsNullOrWhiteSpace(v))
                    {
                        if (v.Trim().StartsWith("\"") == false)
                        {
                            cmdBuilder.Append($" -e \"{v.Trim()}\"");
                        }
                        else
                        {
                            cmdBuilder.Append($" -e {v.Trim()}");
                        }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(memoryLimit))
            {
                cmdBuilder.Append($" -m {memoryLimit}");
            }
            cmdBuilder.Append(' ');
            cmdBuilder.Append(imageName);
            await _cmdRunner.RunForResult(null, cmdBuilder.ToString());
            cmdBuilder.Clear();
        }

        public async Task StartContainer(string containerId)
        {
            await _cmdRunner.RunForResult(null, $"docker start {containerId}");
        }

        public Process StartContainerInProcess(string containerId)
        {
            return _cmdRunner.RunNoOutput(null, $"docker start -a {containerId}");
        }

        public async Task StopContainer(string containerId)
        {
            await _cmdRunner.RunForResult(null, $"docker stop {containerId}");
        }

        public async Task RemoveImage(string imageName)
        {
            await _cmdRunner.RunForResult(null, $"docker rmi {imageName}");
        }

        /// <summary>
        /// 用于清理不再被使用的镜像
        /// </summary>
        /// <returns></returns>
        public async Task PruneImages()
        {
            await _cmdRunner.RunForResult(null, "docker image prune -f");
        }

        public async Task RemoveContainer(string containerId)
        {
            await _cmdRunner.RunForResult(null, $"docker rm {containerId}");
        }
    }
}
