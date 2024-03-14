using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vela.CodeParser
{
    public class FieldInfo : BaseMemberInfo
    {
        public string Type { get; set; }
        public override NodeType NodeType => NodeType.Field;
    }
}
