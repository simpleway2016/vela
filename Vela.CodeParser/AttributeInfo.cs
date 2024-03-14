using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vela.CodeParser
{
    public class AttributeInfo : BaseCodeNodeInfo
    {
        public string Name { get; set; }
        public string[] Arguments { get; set; }
        public override NodeType NodeType => NodeType.Attribute;
    }
}
