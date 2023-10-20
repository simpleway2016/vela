using VelaLib;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;

namespace VelaAgent.Infrastructures
{
    public interface IProcessService
    {
        RunningInfo[] GetCpuUsagePercent(int[] processIds);
        void Kill(int processId);
    }

    public class WindowsProcessService : IProcessService
    {
        public  void Kill(int processId)
        {
            var process = System.Diagnostics.Process.GetProcessById(processId);
            if (process != null)
            {
                if (!process.CloseMainWindow())
                {
                    process.Kill();
                }
            }
        }

        private static long GetPhisicalMemory()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher();   //用于查询一些如系统信息的管理对象 
            searcher.Query = new SelectQuery("Win32_PhysicalMemory ", "", new string[] { "Capacity" });//设置查询条件 
            ManagementObjectCollection collection = searcher.Get();   //获取内存容量 
            ManagementObjectCollection.ManagementObjectEnumerator em = collection.GetEnumerator();

            long capacity = 0;
            while (em.MoveNext())
            {
                ManagementBaseObject baseObj = em.Current;
                if (baseObj.Properties["Capacity"].Value != null)
                {
                    try
                    {
                        capacity += long.Parse(baseObj.Properties["Capacity"].Value.ToString());
                    }
                    catch
                    {
                        return 0;
                    }
                }
            }
            return capacity;
        }

        public RunningInfo[] GetCpuUsagePercent(int[] processIds)
        {
            var sysMemSize = GetPhisicalMemory();
            RunningInfo[] ret = new RunningInfo[processIds.Length];
            TimeSpan[] startTimes = new TimeSpan[processIds.Length];
            Process[] processes = new Process[processIds.Length];


            for (int i = 0; i < processes.Length; i++)
            {
                try
                {
                    if (processIds[i] == 0)
                        continue;

                    processes[i] = Process.GetProcessById(processIds[i]);

                    // 获取进程启动时的总 CPU 时间
                    startTimes[i] = processes[i].TotalProcessorTime;
                }
                catch (Exception)
                {
                    processes[i]?.Dispose();
                    processes[i] = null;
                }
            }

            Thread.Sleep(1000);

            for (int i = 0; i < processes.Length; i++)
            {
                try
                {
                    using Process process = processes[i];
                    if (process == null)
                    {
                        ret[i] = new RunningInfo();
                        continue;
                    }

                    // 获取进程启动时的总 CPU 时间
                    TimeSpan startTime = startTimes[i];


                    // 获取当前时间时的总 CPU 时间
                    TimeSpan endTime = process.TotalProcessorTime;

                    // 计算 CPU 时间的增量
                    TimeSpan cpuUsed = endTime - startTime;

                    var runningInfo = new RunningInfo();
                    // 计算 CPU 占用率（百分比）
                    runningInfo.CpuPercent = Math.Round((cpuUsed.TotalMilliseconds / (Environment.ProcessorCount * 1000)) * 100, 2);
                    runningInfo.MemoryPercent = Math.Round((process.WorkingSet64 * 100.0) / sysMemSize, 2);
                    ret[i] = runningInfo;
                }
                catch (Exception)
                {
                    ret[i] = new RunningInfo();
                }

            }
            return ret;
        }
    }

    public class LinuxProcessService : IProcessService
    {
        private readonly ICmdRunner _cmdRunner;

        public LinuxProcessService(ICmdRunner cmdRunner)
        {
            _cmdRunner = cmdRunner;
        }
        public virtual void Kill(int processId)
        {
            if (System.Diagnostics.Process.Start("kill", $"-15 {processId}").WaitForExit(10000) == false)
            {
                var process = System.Diagnostics.Process.GetProcessById(processId);
                if (process != null)
                {
                    process.Kill();
                }
            }
        }

        /// <summary>
        /// 获取指定进程的cpu使用率
        /// </summary>
        /// <param name="processIds"></param>
        /// <returns></returns>
        public virtual RunningInfo[] GetCpuUsagePercent(int[] processIds)
        {
            RunningInfo[] list = new RunningInfo[processIds.Length];

            for (int i = 0; i < list.Length; i++)
            {
                var runningInfo = list[i] = new RunningInfo();
                if (processIds[i] == 0)
                    continue;

                //ps -p 1149 -o pcpu,pmem
                try
                {
                    var process = _cmdRunner.Run(null, $"ps -p {processIds[i]} -o pcpu,pmem");
                    process.WaitForExit();
                    var ret = process.StandardOutput.ReadToEnd();
                    ret = ret.Split('\n').Select(m => m.Trim()).Where(m => m.Length > 0).ToArray()[1];
                    var strArr = ret.Split(' ').Select(m => m.Trim()).Where(m => m.Length > 0).ToArray();
                    runningInfo.CpuPercent = Math.Round( double.Parse(strArr[0]) / Environment.ProcessorCount , 2);
                    runningInfo.MemoryPercent = Math.Round(double.Parse(strArr[0]) , 2);
                }
                catch
                {
                }
            }
            return list;
        }
    }
}
