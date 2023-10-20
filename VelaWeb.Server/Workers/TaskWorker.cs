using VelaLib;

namespace VelaWeb.Server.Workers
{
    public class TaskWorker : IWorker, IDisposable
    {
        CancellationTokenSource _tokenSource;
        private readonly string _guid;
        private readonly BuildingManager _buildingManager;

        public CancellationToken CancellationToken { get; }
        public TaskWorker(string guid,BuildingManager buildingManager)
        {
            _tokenSource = new CancellationTokenSource();
            CancellationToken = _tokenSource.Token;
            _guid = guid;
            _buildingManager = buildingManager;
        }
        public async Task Kill()
        {
            _tokenSource.Cancel();
        }

        public void Dispose()
        {
            _buildingManager.RemoveWorker(_guid, this);
            _tokenSource.Dispose();
        }
    }
}
