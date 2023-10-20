namespace VelaAgent.DBModels
{
    public class SysDBContext:DBModels.DB.VelaAgent
    {
        public event EventHandler Disposed;
        public SysDBContext() : base("data source=./data.db" , Way.EntityDB.DatabaseType.Sqlite)
        {

        }

        public override void Dispose()
        {
            Disposed?.Invoke(this, null);
            base.Dispose();
        }
    }
}
