using VelaAgent.DBModels;
using VelaLib;

namespace VelaAgent.Infrastructures
{
    public interface IRunningInfoProvider
    {
        Project_RunTypeEnum RunnerType { get; }

        ValueTask<RunningInfo[]> GetCpuUsagePercent(Project[] projects);
    }
}
