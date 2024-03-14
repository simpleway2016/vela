using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vela.CodeParser
{
    public class PropertyInfo : FieldInfo
    {     
        public override NodeType NodeType => NodeType.Property;
    }
}
