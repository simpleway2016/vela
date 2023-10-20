namespace VelaWeb.Server.DBModels
{
    public class SysDBContext:DBModels.DB.VelaServer
    {
        public SysDBContext() : base("data source=./data.db" , Way.EntityDB.DatabaseType.Sqlite)
        {

        }
    }
}
