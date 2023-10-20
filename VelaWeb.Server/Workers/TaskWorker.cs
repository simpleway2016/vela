using VelaLib;

namespace VelaWeb.Server.Workers
{
    public class TaskWorker : IWorker, IDisposable
    {
        CancellationTokenSource _tokenSource;
        private readonly string _guid;

        public CancellationToken CancellationToken { get; }
        public TaskWorker(string guid)
        {
            _tokenSource = new CancellationTokenSource();
            CancellationToken = _tokenSource.Token;
            _guid = guid;
        }
        public async Task Kill()
        {
            _tokenSource.Cancel();
        }

        public void Dispose()
        {
            Global.ServiceProvider.GetRequiredService<BuildingManager>().RemoveWorker(_guid, this);
            _tokenSource.Dispose();
        }
    }
}
