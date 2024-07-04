using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VelaWeb.Server
{
    public class ServiceUpgradeConfigModel
    {
        public string Zip { get; set; }
        /// <summary>
        /// 排除的文件
        /// </summary>
        public string[] ExcludeFiles { get; set; }
    }
}
