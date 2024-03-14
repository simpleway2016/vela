using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vela.CodeParser
{
    /// <summary>
    /// 代码解析接口
    /// </summary>
    public interface ICodeParser
    {
        /// <summary>
        /// 语言
        /// </summary>
        string Language { get; }
        /// <summary>
        /// 解析代码
        /// </summary>
        /// <param name="code">代码</param>
        /// <returns>解析结果</returns>
        BaseCodeNodeInfo Parser(string code);
    }
}
