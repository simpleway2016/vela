using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace VelaService
{
    public interface ICmdRunner
    {
        /// <summary>
        /// 获取运行命令里面的可执行文件名
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        string GetRunFileName(string cmd);
        Process Run(string workingDirectory, string cmd);
        string RunForResult(string workingDirectory, string cmd);
        Process RunInBash(string workingDirectory, string cmd);
        int[] GetChildProcessId(int parentPid);
    }

    public class LinuxCmdRunner : ICmdRunner
    {
        public virtual int[] GetChildProcessId(int parentPid)
        {
            var ppid = parentPid.ToString();

            var ret = this.RunForResult(null, "ps -eo pid,ppid");
            var lines = ret.Split('\n').Where(m => m.Contains("PPID") == false && m.Trim().Length > 0).Select(m => m.Trim()).ToArray();
            List<int> childList = new List<int>();
            foreach (var line in lines)
            {
                var items = line.Split(' ').Where(m => m.Length > 0).Select(m => m.Trim()).ToArray();
                if (items[1] == ppid)
                {
                    childList.Add(int.Parse(items[0]));
                }
            }

            //获取子进程的子进程
            var count = childList.Count;
            for (int i = 0; i < count; i++)
            {
                var mychildPids = GetChildProcessId(childList[i]);
                if (mychildPids.Length > 0)
                {
                    childList.AddRange(mychildPids);
                }
            }

            return childList.ToArray();
        }
        public string GetRunFileName(string cmd)
        {
            cmd = cmd.Trim();

            string filename, args = "";
            if (cmd.StartsWith("\""))
            {
                cmd = cmd.Substring(1);
                filename = cmd.Substring(0, cmd.IndexOf("\""));
            }
            else
            {
                var arr = cmd.Split(' ');
                filename = arr[0];
            }

            if (filename.StartsWith("./"))
                filename = filename.Substring(2);
            return filename;
        }
        public Process Run( string workingDirectory, string cmd)
        {
            cmd = cmd.Trim();
            if (workingDirectory != null && workingDirectory.StartsWith("./"))
            {
                workingDirectory = Path.GetFullPath(workingDirectory, Environment.CurrentDirectory);
            }

            string filename, args = "";
            if (cmd.StartsWith("\""))
            {
                cmd = cmd.Substring(1);
                filename = cmd.Substring(0, cmd.IndexOf("\""));

                if (cmd.Length > filename.Length + 2)
                {
                    args = cmd.Substring(cmd.IndexOf("\"") + 1).Trim();
                }
            }
            else
            {
                var arr = cmd.Split(' ');
                filename = arr[0];
                if (arr.Length > 1)
                {
                    args = cmd.Substring(filename.Length).Trim();
                }
            }
            filename = filename.Trim();
            if (filename.StartsWith("./"))
            {
                filename = Path.GetFullPath(filename, workingDirectory);

            }
            else
            {
                if (filename.Contains(".") && filename.Contains("/") == false)
                {
                    //有扩展名，应该不是全局命令
                    filename = Path.Combine(workingDirectory, filename);
                }
            }

            ProcessStartInfo processStartInfo = new ProcessStartInfo(filename , args) { 
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardErrorEncoding = Encoding.UTF8,
                StandardInputEncoding = Encoding.UTF8,
                StandardOutputEncoding = Encoding.UTF8,
            };
#if DEBUG
            if (OperatingSystem.IsLinux())
            {
                //vs在WSL运行时，会携带NUGET_FALLBACK_PACKAGES这些环境变量，导致项目还原包失败
                processStartInfo.EnvironmentVariables.Remove("NUGET_FALLBACK_PACKAGES");
                processStartInfo.EnvironmentVariables.Remove("NUGET_PACKAGES");
            }
#endif
            if (workingDirectory != null)
            {
                processStartInfo.WorkingDirectory = workingDirectory;
            }
            return Process.Start(processStartInfo);
        }

        public virtual Process RunInBash(string workingDirectory, string cmd)
        {
            if (string.IsNullOrWhiteSpace(cmd))
            {
                return null;
            }
            var folder = "./cmdTemp";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            else
            {
                //删除老文件
                IOUtility.DeleteOldFiles(folder, 2);
            }
            var shFileName = Path.GetFullPath($"{folder}/{Guid.NewGuid().ToString("N")}.sh", AppDomain.CurrentDomain.BaseDirectory);

            StringBuilder shBuilder = new StringBuilder();
            shBuilder.Append("#!/bin/bash\n");
            shBuilder.Append("exit_code=0\n");

            var cmds = cmd.Split('\n').Select(m => m.Trim()).Where(m => m.Length > 0).ToArray();
            for(int i = 0; i < cmds.Length; i ++)
            {
                var cmdStringItem = cmds[i];

                if(i > 0)
                {
                    shBuilder.Append("exit_code=$?\n");
                }

                shBuilder.Append("if [ $exit_code -ne 0 ]; then\n");
                shBuilder.Append("  exit $exit_code\n");
                shBuilder.Append("else\n  ");
                shBuilder.Append($"echo \"{cmdStringItem.Replace("\"","\\\"")}\"\n");
                shBuilder.Append(cmdStringItem);
                shBuilder.Append('\n');
                shBuilder.Append("fi\n\n");
            }

            //这里需要生成不带bom的sh文件，所以不能用File.WriteAllText
            File.WriteAllBytes(shFileName, Encoding.UTF8.GetBytes(shBuilder.ToString()));

            return this.Run(workingDirectory, $"bash \"{shFileName}\"");
        }

        public string RunForResult(string workingDirectory, string cmd)
        {
            var p = Run(workingDirectory, cmd);
            p.WaitForExit();
            var text = new string[] { p.StandardOutput.ReadToEnd(), p.StandardError.ReadToEnd() };
            var arr = text.Where(m => !string.IsNullOrWhiteSpace(m)).ToArray();
            var ret = string.Join("\n", arr);

            if (p.ExitCode != 0)
            {
              
                throw new Exception(ret);
            }
            return ret;
        }
    }

    public class WindowsCmdRunner : LinuxCmdRunner
    {
        public override int[] GetChildProcessId(int parentPid)
        {
            List<int> childList = new List<int>();
            using (var searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + parentPid))
            {
                using var moc = searcher.Get();
                foreach (var mo in moc)
                {
                    childList.Add(Convert.ToInt32(mo["ProcessID"]));
                }
            }

            //获取子进程的子进程
            var count = childList.Count;
            for (int i = 0; i < count; i++)
            {
                var mychildPids = GetChildProcessId(childList[i]);
                if (mychildPids.Length > 0)
                {
                    childList.AddRange(mychildPids);
                }
            }

            return childList.ToArray();
        }
        public override Process RunInBash(string workingDirectory, string cmd)
        {
            if (string.IsNullOrWhiteSpace(cmd))
            {
                return null;
            }
            var folder = "./cmdTemp";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            else
            {
                //删除老文件
                IOUtility.DeleteOldFiles(folder, 2);
            }

            var batFilePath = Path.GetFullPath($"{folder}/{Guid.NewGuid().ToString("N")}.bat", AppDomain.CurrentDomain.BaseDirectory);

            StringBuilder shBuilder = new StringBuilder();
            shBuilder.Append("@echo off\r\n");
            shBuilder.Append("@chcp 65001\r\n");
            shBuilder.Append("set exitcode=0\r\n");

            var cmds = cmd.Split('\n').Select(m => m.Trim()).Where(m => m.Length > 0).ToArray();
            for (int i = 0; i < cmds.Length; i++)
            {
                var cmdStringItem = cmds[i];

                if (i > 0)
                {
                    shBuilder.Append("set exitcode=%errorlevel%\r\n");
                }

                shBuilder.Append("if %exitcode% neq 0 (\r\n");
                shBuilder.Append("  exit /b %exitcode%\r\n");
                shBuilder.Append(") else (\r\n  ");
                shBuilder.Append($"echo {cmdStringItem}\r\n");
                shBuilder.Append(cmdStringItem);
                shBuilder.Append('\n');
                shBuilder.Append(")\n\n");
            }

            File.WriteAllBytes(batFilePath, Encoding.UTF8.GetBytes(shBuilder.ToString()));

            return this.Run(workingDirectory, $"cmd /c \"{batFilePath}\"");
        }
    }
}
