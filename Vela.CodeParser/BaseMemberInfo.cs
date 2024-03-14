using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vela.CodeParser
{
    public class BaseMemberInfo: BaseCodeNodeInfo
    {
       
        public IEnumerable<string> Modifiers { get; set; }
        public string Name { get; set; }
        public CaptionInfo Caption { get; set; }
       
    }
   
}
