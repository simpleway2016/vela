using System.Collections.Concurrent;
using VelaLib;

namespace VelaAgent.Infrastructures
{
    public class ProjectTtyWorker : ConcurrentDictionary<string,TtyWorker>
    {
    }
}
