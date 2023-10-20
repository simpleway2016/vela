using VelaAgent.DBModels;

namespace VelaAgent.Infrastructures
{
    public interface IProjectRunner
    {
        Project Project { get; set; }
        IInfoOutput InfoOutput { get; set; }
        Project_RunTypeEnum RunnerType { get; }

        Task DeleteProject();
        bool KeepAlive();

        Task Start();
        Task Stop();
    }
}
