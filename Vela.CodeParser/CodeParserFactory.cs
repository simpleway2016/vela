using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vela.CodeParser;

namespace Vela.CodeParser
{
    public class CodeParserFactory
    {
        IEnumerable<ICodeParser> _codeParsers;
        public CodeParserFactory(IServiceProvider serviceProvider)
        {
            _codeParsers = serviceProvider.GetServices<ICodeParser>();
        }

        /// <summary>
        /// 创建指定语言的ICodeParser
        /// </summary>
        /// <param name="language">语言，如：CSharp</param>
        /// <returns></returns>
        public ICodeParser? CreateCodeParser(string language)
        {
            return _codeParsers.FirstOrDefault(m=>string.Equals(m.Language , language , StringComparison.OrdinalIgnoreCase));
        }

        public string[] GetAllLanguages()
        {
            return _codeParsers.Select(m => m.Language).OrderBy(m=>m).ToArray();
        }
    }

  
}

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CodeParserFactoryExtension
    {
        public static IServiceCollection RegisterCodeParser<T>(this IServiceCollection services) where T : ICodeParser
        {
            services.AddSingleton<CodeParserFactory>();
            services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(ICodeParser), typeof(T)));
            return services;
        }
    }
}
