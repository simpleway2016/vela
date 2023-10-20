using System.Collections.Concurrent;
using VelaWeb.Server.DBModels;
using VelaWeb.Server.Models;

namespace VelaWeb.Server
{
    public class AgentsManager
    {
        private readonly ProjectCenter _projectCenter;
        ConcurrentDictionary<long, AgentModel> _agentModels = new ConcurrentDictionary<long, AgentModel>();
        public AgentsManager(ProjectCenter projectCenter)
        {
            _projectCenter = projectCenter;
        }

        public void Init()
        {
            using var db = new SysDBContext();
            var agents = db.Agent.Select(m => new AgentModel
            {
                id = m.id.Value,
                Address = m.Address,
                Port = m.Port
            }).ToArray();

            foreach (var agent in agents)
            {
                _agentModels[agent.id] = agent;
                agent.Init();
            }
        }

        public IEnumerable<AgentModel> GetAllAgents()
        {
            return _agentModels.Select(m => m.Value);
        }

        public void OnNewAgent(Agent agentItem)
        {
            var model = _agentModels[agentItem.id.Value] = new AgentModel()
            {
                id = agentItem.id.Value,
                Address = agentItem.Address,
                Port = agentItem.Port
            };
            model.Init();

            _projectCenter.OnNewAgent(agentItem);

        }
        public void OnRemoveAgent(long agentId)
        {
            if (_agentModels.TryRemove(agentId, out AgentModel agent))
            {
                agent.Dispose();

                _projectCenter.OnRemoveAgent(agentId);
            }
        }
    }
}
