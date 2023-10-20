using Docker.DotNet.Models;
using VelaAgent.DBModels;
using VelaLib;

namespace VelaAgent.Infrastructures.RunningInfoProviders
{
    public class DockerRunningInfoProvider : IRunningInfoProvider
    {
        private readonly IDockerEngine _dockerEngine;

        public Project_RunTypeEnum RunnerType => Project_RunTypeEnum.Docker;

        public DockerRunningInfoProvider(IDockerEngine dockerEngine)
        {
            _dockerEngine = dockerEngine;
        }
        public async ValueTask<RunningInfo[]> GetCpuUsagePercent(Project[] projects)
        {
            var statsObjs = await _dockerEngine.GetRunningContainerStates();

            var cpuInfos = new RunningInfo[projects.Length];
         
            for(int i = 0; i < projects.Length; i ++)
            {
                var cpuinfo = cpuInfos[i] = new RunningInfo();
                cpuinfo.Guid = projects[i].Guid;

                var state = statsObjs.FirstOrDefault(m=>m.Id == projects[i].DockerContainerId);
                if(state != null)
                {
                    cpuinfo.ProcessId = -2;
                    cpuinfo.ContainerId = projects[i].DockerContainerId;
                    cpuinfo.CpuPercent = state.CpuPerc;
                    cpuinfo.MemoryPercent = state.MemPerc;
                }
            }

            return cpuInfos;
        }
    }
}
