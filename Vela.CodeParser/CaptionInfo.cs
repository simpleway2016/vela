using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vela.CodeParser
{
    public class CaptionInfo
    {
        public string Main { get; set; }
        public IEnumerable<ParameterCaptionInfo> Parameters { get; set; } 
        public IEnumerable<ExceptionCaptionInfo> Exceptions { get; set; } 
        public string Example { get; set; }
        public string Return { get; set; }
    }
    public class ExceptionCaptionInfo
    {
        public string Cref { get; set; }
    }
    public class ParameterCaptionInfo
    {
        public string Name { get; set; }
        public string Comment { get; set; }
    }
}
