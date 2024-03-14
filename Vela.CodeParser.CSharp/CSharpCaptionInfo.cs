using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Vela.CodeParser;

namespace Vela.CodeParser.CSharp
{
    public class CSharpCaptionInfo : CaptionInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="comment"></param>
        public CSharpCaptionInfo(string comment) {
            var content = comment.Trim();
            if (content.StartsWith("<"))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml($"<root>{content}</root>");

                this.Main = doc.DocumentElement.SelectSingleNode("summary").InnerText.Trim();
                var parameNodes = doc.DocumentElement.SelectNodes("param");
                foreach( XmlElement pnode in parameNodes)
                {
                    ((IList)(this.Parameters??=new List<ParameterCaptionInfo>())).Add(new ParameterCaptionInfo { 
                        Comment = pnode.InnerText.Trim(),
                        Name = pnode.GetAttribute("name").Trim()
                    });
                }

                var exceptionNodes = doc.DocumentElement.SelectNodes("exception");
                foreach (XmlElement pnode in exceptionNodes)
                {
                    ((IList)(this.Exceptions ??= new List<ExceptionCaptionInfo>())).Add(new ExceptionCaptionInfo
                    {
                        Cref = pnode.GetAttribute("cref").Trim()
                    });
                }

                this.Return = doc.DocumentElement.SelectSingleNode("returns")?.InnerText.Trim();//
                this.Example = doc.DocumentElement.SelectSingleNode("example")?.InnerText.Trim();

            }
            else
            {
                this.Main = content;
            }
        }
    }
}
