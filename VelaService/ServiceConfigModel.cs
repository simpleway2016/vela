using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VelaService
{
    public class ServiceConfigModel
    {
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public string ExecStart { get; set; }
        public string WorkDir { get; set; }
        public bool AddUserToDockerGroup { get; set; }
        public int Kill { get; set; } = -15;
    }
    public class ServiceUpgradeConfigModel
    {
        public string Zip { get; set; }
        /// <summary>
        /// 排除的文件
        /// </summary>
        public string[] ExcludeFiles { get; set; }
    }
}
