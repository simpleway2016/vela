using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text;
using Vela.CodeParser;
using Vela.CodeParser.CSharp;
namespace Vela.UnitTest
{
    public class CSharpCodeParserTest
    {
        [Fact]
        public void Test1()
        {
            
            ServiceCollection services = new ServiceCollection();
            services.RegisterCodeParser<CSharpCodeParser>();
           
            var sp = services.BuildServiceProvider();
            var codeParser = sp.GetService<CodeParserFactory>().CreateCodeParser("csharp");

            var code = File.ReadAllText("test.txt", Encoding.UTF8);
            var ret = codeParser.Parser(code);
        }
    }
}