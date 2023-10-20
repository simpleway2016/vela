using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VelaLib
{
    public class NormalClosureException : Exception
    {
        public NormalClosureException(string msg):base(msg) { }
    }
}
