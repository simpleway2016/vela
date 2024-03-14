using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vela.CodeParser
{
    public class MethodInfo : BaseMemberInfo
    {
        public IEnumerable<ParameterInfo> Parameters { get; set; }
        public string ReturnType { get; set; }
        public override NodeType NodeType => NodeType.Method;
    }
}
