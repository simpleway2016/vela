using System.Collections.Concurrent;

namespace VelaAgent.Infrastructures
{
    public class ProjectRunnerHelper
    {
        static ConcurrentDictionary<string,bool> ProjectStartFlags = new ConcurrentDictionary<string,bool>();
        public static bool SetProjectStarting(string guid)
        {
            if (ProjectStartFlags.TryAdd(guid, true))
                return true;
            return false;
        }

        public static void SetProjectStartCompleted(string guid)
        {
            ProjectStartFlags.TryRemove(guid, out _);
        }
    }
}
