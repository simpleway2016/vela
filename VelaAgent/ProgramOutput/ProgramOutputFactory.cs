using VelaAgent.KeepAlive;
using Way.Lib;

namespace VelaAgent.ProgramOutput
{
    public class ProgramOutputFactory
    {
        public IProgramOutput CreaateProgramOutput(KeepProcessAlive keepProcessAlive)
        {
            if(keepProcessAlive.Project == null)
            {
                throw new Exception("Project is null");
            }
            if(keepProcessAlive.Project.RunType == DBModels.Project_RunTypeEnum.Docker && !string.IsNullOrEmpty(keepProcessAlive.Project.DockerContainerId))
            {
                return new DockerProgramOutput(keepProcessAlive.Project.DockerContainerId);
            }
            return null;
        }
    }
}
