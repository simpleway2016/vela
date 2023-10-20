using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelaLib;

namespace VelaWeb.Server
{
    public class BuildingManager
    {
        List<RequestBuilding> _requestingList = new List<RequestBuilding>();
        ConcurrentDictionary<string, ConcurrentDictionary<IWorker, bool>> _workers = new ConcurrentDictionary<string, ConcurrentDictionary<IWorker, bool>>();
        public BuildingManager()
        {

        }

        public string[] GetBuildingProjects()
        {
            List<string> ret = new List<string>();
            try
            {
                for (int i = 0; i < _requestingList.Count; i++)
                {
                    var item = _requestingList[i];
                    if (item != null)
                    {
                        ret.Add(item.ProjectName);
                    }
                }
            }
            catch
            {

            }
            return ret.ToArray();
        }

        public string GetRequestingComments()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                for (int i = 0; i < _requestingList.Count; i++)
                {
                    var item = _requestingList[i];
                    if (item != null)
                    {
                        if (sb.Length > 0)
                            sb.Append(" ; ");

                        sb.Append(item.Comment);
                    }
                }
            }
            catch
            {

            }
            return sb.ToString();
        }

        public void AddWorker(string guid, IWorker worker)
        {
            var dict = _workers.GetOrAdd(guid, s => new ConcurrentDictionary<IWorker, bool>());
            dict.TryAdd(worker, true);
        }

        public IEnumerable<IWorker>? GetWorkers(string guid)
        {
            if (_workers.TryGetValue(guid, out ConcurrentDictionary<IWorker, bool> dict))
            {
                return dict.Select(m => m.Key);
            }
            return null;
        }

        public void RemoveWorker(string guid, IWorker worker)
        {
            if (_workers.TryGetValue(guid, out ConcurrentDictionary<IWorker, bool> dict))
            {
                dict.TryRemove(worker, out _);
            }
        }

        public bool TryAddRequest(RequestBuilding requestBuilding)
        {
            lock (_requestingList)
            {
                if (_requestingList.Count >= Global.MaxRequestBuilding)
                    return false;
                _requestingList.Add(requestBuilding);
                return true;
            }
        }

        public void EndRequest(RequestBuilding requestBuilding)
        {
            lock (_requestingList)
            {
                if (_requestingList.Contains(requestBuilding))
                {
                    _requestingList.Remove(requestBuilding);
                }
            }
        }
    }

    public class RequestBuilding : IDisposable
    {
        private BuildingManager _buildingManager;

        public string ProjectName { get; }
        public string Comment { get; }

        public RequestBuilding(BuildingManager buildingManager, string projectName, string comment)
        {
            _buildingManager = buildingManager;
            ProjectName = projectName;
            Comment = comment;
        }

        public void Dispose()
        {
            if (_buildingManager != null)
            {
                _buildingManager.EndRequest(this);
                _buildingManager = null;
            }
        }
    }
}
