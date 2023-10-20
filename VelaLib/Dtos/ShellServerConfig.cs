using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VelaLib.Dtos
{
    public class ShellServerConfig
    {
        public int MinPort { get; set; } = 20000;
        public int MaxPort { get; set; } = 30000;
    }
}
