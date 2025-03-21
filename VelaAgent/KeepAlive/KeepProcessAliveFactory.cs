using VelaAgent.DBModels;
using System.Collections.Concurrent;

namespace VelaAgent.KeepAlive
{
    public class KeepProcessAliveFactory
    {
        ConcurrentDictionary<string, KeepProcessAlive> _existItems = new ConcurrentDictionary<string, KeepProcessAlive>();
        public KeepProcessAliveFactory()
        {

        }

        public async void Init()
        {
            using var db = new SysDBContext();
            var projects = db.Project.ToArray();
            foreach (var project in projects)
            {
                if (project.IsFirstUpload == false && project.IsStopped == false)
                {
                    if (!string.IsNullOrWhiteSpace(project.RunCmd) && project.ProcessId != null || project.RunType != Project_RunTypeEnum.Program)
                    {
                        var item = Create(project.Guid);
                        if (item.Keep() == false)
                        {
                            try
                            {
                                if (project.IsStopped)
                                {
                                    project.ProcessId = null;
                                    db.Update(project);
                                }
                                else
                                {
                                    await item.Start();
                                }
                            }
                            catch (Exception)
                            {
                                project.ProcessId = null;
                                db.Update(project);
                            }
                        }
                    }
                }
            }
        }

        public Project[] GetAllProjects()
        {
            return _existItems.Values.Select(m => m.Project).ToArray();
        }

        public void UpdateProject(Project project)
        {
            if (project != null)
            {
                var obj = _existItems.Values.FirstOrDefault(m => m.Project.Guid == project.Guid);
                if (obj != null)
                {
                    obj.Project = project;
                }
            }
        }

        public KeepProcessAlive Create(string projectGuid)
        {
            return _existItems.GetOrAdd(projectGuid, key =>
            {
                return new KeepProcessAlive(projectGuid);
            });
        }

        public void RemoveCache(string projectGuid)
        {
            _existItems.TryRemove(projectGuid, out KeepProcessAlive keepProcessAlive);
        }
    }
}
