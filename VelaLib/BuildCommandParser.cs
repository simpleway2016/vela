using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VelaLib
{
    public class BuildCommandParser
    {
        /// <summary>
        /// 编译前命令
        /// </summary>
        public string[] BeforeCommands { get;private set; }
        /// <summary>
        /// 编译命令
        /// </summary>
        public string[] Commands { get; private set; }
        /// <summary>
        /// 编译后命令
        /// </summary>
        public string[] AfterCommands { get; private set; }
        public BuildCommandParser(string buildCmd)
        {
            if (string.IsNullOrWhiteSpace(buildCmd))
            {
                return;
            }
            var buildCommands = buildCmd.Split('\n').Select(m => m.Trim()).Where(m => m.Length > 0 && m.StartsWith("#") == false).ToArray();
            for(int i = 0; i < buildCommands.Length; i ++)
            {
                if (buildCommands[i] == "--before")
                {
                    this.BeforeCommands = buildCommands.Take(i).ToArray();
                    buildCommands = buildCommands.Skip(i + 1).ToArray();
                    break;
                }
            }

            for (int i = 0; i < buildCommands.Length; i++)
            {
                if (buildCommands[i] == "--after")
                {
                    this.Commands = buildCommands.Take(i).ToArray();
                    buildCommands = buildCommands.Skip(i + 1).ToArray();
                    break;
                }
            }

            if(this.Commands != null)
            {
                this.AfterCommands = buildCommands;
            }
            else
            {
                this.Commands = buildCommands;
            }
        }
    }
}
