using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vela.CodeParser
{
    public class EnumMemberInfo : BaseMemberInfo
    {
        public string Value { get; set; }
        public override NodeType NodeType => NodeType.EnumMember;
    }
}
