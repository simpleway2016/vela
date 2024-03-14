using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vela.CodeParser
{
    public class BaseCodeNodeInfo
    {
        public virtual NodeType NodeType { get; } = NodeType.None;
        public IEnumerable<AttributeInfo> Attributes { get; set; }
        public IEnumerable<BaseCodeNodeInfo> Items { get; set; }
    }

    public enum NodeType
    {
        None = 0,
        Namespace = 1,
        Class = 2,
        Method = 3,
        Property = 4,
        Field = 5,
        MethodParameter = 6,
        Attribute = 7,
        Enum = 8,
        Interface = 9,
        EnumMember = 10
    }


}
