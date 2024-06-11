//using Docker.DotNet.Models;
//using Docker.DotNet;
//using System.Diagnostics;
//using VelaAgent.Dtos;
//using VelaLib;
//using System.ComponentModel;
//using System.Threading;
//using Way.Lib;


//namespace VelaAgent.Infrastructures
//{
//    public class LibDockerEngine : IDockerEngine
//    {
//        private readonly ICmdRunner _cmdRunner;

//        public LibDockerEngine(ICmdRunner cmdRunner)
//        {
//            _cmdRunner = cmdRunner;
//        }

//        public async Task<DockerContainer[]> GetContainers()
//        {
//            //var text = await _cmdRunner.RunForResult(null, $"docker ps");

//            using (DockerClient client = new DockerClientConfiguration().CreateClient())
//            {
//                // 查询正在运行的容器
//                var containers = await client.Containers.ListContainersAsync(new ContainersListParameters { All = true });

//                var ret = (from m in containers
//                           select new DockerContainer
//                           {
//                               Id = m.ID,
//                               ImageName = m.Image,
//                               //Name = m.Names.FirstOrDefault()
//                           }).ToArray();
//                foreach (var item in ret)
//                {
//                    //if (item.Name.StartsWith("/"))
//                    //    item.Name = item.Name.Substring(1);
//                }
//                return ret;
//            }
//        }

//        public async Task<DockerContainerState[]> GetRunningContainerStates()
//        {
//            var json = await _cmdRunner.RunForResult(null, "docker stats --no-stream --format \"{{ json . }}\"");
//            var jsonArr = json.Split('\n');
//            List<DockerContainerState> list = new List<DockerContainerState>();
//            foreach (var jsonItem in jsonArr)
//            {
//                if (string.IsNullOrWhiteSpace(jsonItem))
//                    continue;
//                var obj = jsonItem.FromJson<DockerStatsJsonObject>();
//                var stateObj = new DockerContainerState
//                {
//                    Id = obj.ID,
//                };
//                try
//                {
//                    stateObj.CpuPerc = double.Parse(obj.CPUPerc.Replace("%", ""));
//                }
//                catch
//                {
//                }
//                try
//                {
//                    stateObj.MemPerc = double.Parse(obj.MemPerc.Replace("%", ""));
//                }
//                catch
//                {
//                }
//                list.Add(stateObj);
//            }

//            return list.ToArray();
//        }

//        public async Task<string[]> GetImages()
//        {
//            using (DockerClient client = new DockerClientConfiguration().CreateClient())
//            {
//                var images = await client.Images.ListImagesAsync(new ImagesListParameters { All = true });
//                List<string> ret = new List<string>();
//                foreach (var image in images)
//                {
//                    ret.AddRange(image.RepoTags);
//                }
//                return ret.ToArray();
//            }
//        }

//        public async Task StartContainer(string containerId)
//        {
//            using (DockerClient client = new DockerClientConfiguration().CreateClient())
//            {
//                // 启动容器
//                await client.Containers.StartContainerAsync(containerId, new ContainerStartParameters());
//            }
//        }
//        public async Task StopContainer(string containerId)
//        {
//            using (DockerClient client = new DockerClientConfiguration().CreateClient())
//            {
//                // 启动容器
//                await client.Containers.StopContainerAsync(containerId, new ContainerStopParameters());
//            }
//        }
//        public async Task RunImage(string imageName, string containerName, string programPath, IEnumerable<string> portMaps)
//        {
//            var portBindings = new Dictionary<string, IList<PortBinding>>();
//            if (portMaps != null)
//            {
//                foreach (var portmap in portMaps)
//                {
//                    if (portmap == null)
//                        continue;

//                    var arr = portmap.Split(':');
//                    if (arr.Length != 2)
//                        continue;

//                    var key = $"{arr[1].Trim()}/tcp";
//                    if (portBindings.TryGetValue(key, out IList<PortBinding> list))
//                    {

//                    }
//                    else
//                    {
//                        list = new List<PortBinding>();
//                        portBindings[key] = list;
//                    }
//                    list.Add(new PortBinding { HostPort = arr[0].Trim() });

//                }
//            }

//            using (DockerClient client = new DockerClientConfiguration().CreateClient())
//            {
//                // 定义容器的配置
//                CreateContainerParameters containerParams = new CreateContainerParameters
//                {
//                    Image = imageName,
//                    Name = containerName,
//                    Tty = false,
//                    HostConfig = new HostConfig
//                    {
//                        RestartPolicy = new RestartPolicy()
//                        {
//                            Name = RestartPolicyKind.OnFailure,
//                            MaximumRetryCount = 3
//                        },
//                        Binds = new List<string>
//                    {
//                        $"{programPath}:/vela/app"
//                    },
//                        PortBindings = portBindings
//                    }
//                };

//                // 创建容器
//                CreateContainerResponse containerResponse = await client.Containers.CreateContainerAsync(containerParams);

//                // 启动容器
//                await client.Containers.StartContainerAsync(containerResponse.ID, new ContainerStartParameters());
//            }
//        }

//        public async Task Build(IInfoOutput infoOutput, string projectPath, string imageName)
//        {
//            using var process = _cmdRunner.Run(projectPath, $"docker build -t {imageName} .");
//            readOutput(process, infoOutput);
//            readError(process, infoOutput);
//            await process.WaitForExitAsync();
//            if (process.ExitCode != 0)
//            {
//                throw new Exception("docker编译失败");
//            }
//        }

//        async void readOutput(Process process, IInfoOutput infoOutput)
//        {
//            while (true)
//            {
//                var line = process.StandardOutput.ReadLine();
//                if (line == null)
//                    break;

//                if (line.Length > 0)
//                {
//                    infoOutput?.Output(line);
//                }
//            }
//        }
//        async void readError(Process process, IInfoOutput infoOutput)
//        {
//            while (true)
//            {
//                var line = process.StandardError.ReadLine();
//                if (line == null)
//                    break;

//                if (line.Length > 0)
//                {
//                    infoOutput?.Output(line);
//                }
//            }
//        }

//        public Task RemoveImage(string imageName)
//        {
//            throw new NotImplementedException();
//        }

//        public Task RemoveContainer(string containerId)
//        {
//            throw new NotImplementedException();
//        }

//        public Task PruneImages()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
