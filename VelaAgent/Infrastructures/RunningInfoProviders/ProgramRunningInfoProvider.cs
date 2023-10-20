using VelaAgent.DBModels;
using VelaLib;

namespace VelaAgent.Infrastructures.RunningInfoProviders
{
    public class ProgramRunningInfoProvider : IRunningInfoProvider
    {
        private readonly IProcessService _processService;
        public Project_RunTypeEnum RunnerType => Project_RunTypeEnum.Program;

        public ProgramRunningInfoProvider(IProcessService processService)
        {
            _processService = processService;
        }
        public async ValueTask<RunningInfo[]> GetCpuUsagePercent(Project[] projects)
        {
            var cpuInfos = _processService.GetCpuUsagePercent(projects.Select(m => (int)m.ProcessId.GetValueOrDefault()).ToArray());
         
            for(int i = 0; i < projects.Length; i ++)
            {
                var cpuinfo = cpuInfos[i];
                cpuinfo.Guid = projects[i].Guid;
                cpuinfo.ProcessId = projects[i].ProcessId.GetValueOrDefault();
            }

            return cpuInfos;
        }
    }
}
