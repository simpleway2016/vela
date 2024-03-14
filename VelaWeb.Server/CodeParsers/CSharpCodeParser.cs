using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;
using Vela.CodeParser;

namespace VelaWeb.Server.CodeParsers
{
    public class JsonCodeParser : ICodeParser
    {
        public string Language => "Json";

        public BaseCodeNodeInfo Parser(string code)
        {
            return new JsonCodeNodeInfo(code);
        }
    }

    class JsonCodeNodeInfo : BaseCodeNodeInfo
    {
        private readonly string _code;

        public JsonCodeNodeInfo(string code)
        {
            _code = code;
        }
        public override string ToString()
        {
            return _code;
        }
    }
}
