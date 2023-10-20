using VelaAgent.Infrastructures;

namespace VelaAgent.ProgramOutput
{
    public interface IProgramOutput:IDisposable
    {
        Task StartOutput(IInfoOutput infoOutput,int preLines);
    }
}
