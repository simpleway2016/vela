using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VelaLib
{
    public class RunningInfo
    {
        public double CpuPercent { get; set; }
        public double MemoryPercent { get; set; }
        public string Guid { get; set; }
        public long ProcessId { get; set; }
        public string ContainerId { get; set; }
    }
}
