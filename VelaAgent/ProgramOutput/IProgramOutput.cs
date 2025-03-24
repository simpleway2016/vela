using VelaAgent.DBModels;
using VelaAgent.Infrastructures;

namespace VelaAgent.ProgramOutput
{
    public interface IProgramOutput:IDisposable
    {
        Task StartOutput(Project project, IInfoOutput infoOutput,int preLines);
    }
}
