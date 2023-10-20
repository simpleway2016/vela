using VelaWeb.Server.DBModels;

namespace VelaWeb.Server.Dtos
{
    public class LogResponse : Logs
    {
        public string UserName { get; set; }
    }
}
