using VelaAgent.DBModels;
using VelaAgent.Infrastructures;
using System.Diagnostics;
using System.Reflection;
using System.Net.WebSockets;
using VelaLib;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using VelaAgent.Dtos;

namespace VelaAgent.KeepAlive
{
    /// <summary>
    /// 让程序保持运行
    /// </summary>
    public class KeepProcessAlive
    {
        public IInfoOutput InfoOutput
        {
            get
            {
                return _projectRunner.InfoOutput;
            }
            set
            {
                _projectRunner.InfoOutput = value;
            }
        }
        public Project Project
        {
            get
            {
                return _projectRunner.Project;
            }
            set
            {
                _projectRunner.Project = value;
                if (_projectRunner.RunnerType != value.RunType)
                {
                    _projectRunner = _projectRunnerFactory.CreateProjectRunner(value);
                }
            }
        }

        ProjectRunnerFactory _projectRunnerFactory;
        IProjectRunner _projectRunner;

        public KeepProcessAlive(string projectGuid)
        {
            _projectRunnerFactory = Global.ServiceProvider.GetRequiredService<ProjectRunnerFactory>();

            using var db = new SysDBContext();
            var project = db.Project.FirstOrDefault(m => m.Guid == projectGuid);
            if (project == null)
                throw new Exception($"项目{project.Name}不存在");

            _projectRunner = _projectRunnerFactory.CreateProjectRunner(project);
        }

        /// <summary>
        /// 保持进程运行
        /// </summary>
        public bool Keep()
        {
            if (_projectRunner == null)
            {

                return false;
            }
            return _projectRunner.KeepAlive();
        }

        /// <summary>
        /// 启动程序，并保持运行
        /// </summary>
        public Task Start()
        {
            if (_projectRunner == null)
            {
                return Task.CompletedTask;
            }
            return _projectRunner.Start();
        }


        public Task DeleteProject()
        {
            if (_projectRunner == null)
            {
                return Task.CompletedTask;
            }
            return _projectRunner.DeleteProject();
        }

        public Task Stop()
        {
            if (_projectRunner == null)
            {
                return Task.CompletedTask;
            }
            return _projectRunner.Stop();
        }

    }
}
