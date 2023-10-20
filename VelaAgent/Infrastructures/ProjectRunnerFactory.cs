using VelaAgent.DBModels;
using VelaAgent.Infrastructures.ProjectRunners;

namespace VelaAgent.Infrastructures
{
    public class ProjectRunnerFactory
    {
        public IProjectRunner CreateProjectRunner(Project project)
        {
            if (project != null)
            {
                if (project.RunType == Project_RunTypeEnum.Program)
                {
                    return new ProgramRunner(project);
                }
                else if (project.RunType == Project_RunTypeEnum.Docker)
                {
                    return new DockerRunner(project);
                }
            }
            return null;
        }
    }
}
