using System.Text.Json.Serialization;

namespace VelaWeb.Server.Dtos
{
    public class LogFile
    {
        public DateTime CreateTime { get; set; }
        public string FileName { get; set; }
        public long Length { get; set; }
    }
}
