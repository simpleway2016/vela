using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VelaService
{
    public interface ISystemService
    {
        /// <summary>
        /// 把当前程序注册为系统服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="desc"></param>
        /// <param name="cmd">服务启动时执行的命令</param>
        void Register(string username, string serviceName, string desc,string workdir, string cmd);
    }
    public class LinuxSystemService : ISystemService
    {
        /// <summary>
        /// 把当前程序注册为系统服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="cmd">服务启动时执行的命令</param>
        public void Register(string username, string serviceName,string desc,string workdir,string cmd)
        {
            if(workdir != null && workdir.EndsWith("/"))
            {
                workdir = workdir.Substring(0, workdir.Length - 1);
            }
            var root = "/etc/systemd/system";
            var filepath = Path.Combine(root, $"{serviceName}.service");

            bool needReload = false;
            if (File.Exists(filepath))
            {
                needReload = true;
            }
            var content = $@"[Unit]
Description={desc}
After=network.target

[Service]
Type=simple
WorkingDirectory={workdir}
ExecStart={cmd}
Restart=always
User={username}

[Install]
WantedBy=multi-user.target

";
            File.WriteAllText( filepath, content , Encoding.UTF8);
            if (needReload)
            {
                Process.Start("systemctl", $"stop {serviceName}").WaitForExit();
                Process.Start("systemctl", $"disable {serviceName}").WaitForExit();
                Process.Start("systemctl", $"daemon-reload").WaitForExit();
            }
            Process.Start("systemctl" , $"enable {serviceName}.service");

          
        }
    }
}
