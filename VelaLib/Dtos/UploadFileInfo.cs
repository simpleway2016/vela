using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VelaLib.Dtos
{
    public class UploadFileInfo
    {
        public long Length { get; set; }
        public string MD5 { get; set; }
        public string Path { get; set; }
    }
}
