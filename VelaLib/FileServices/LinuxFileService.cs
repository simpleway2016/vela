using VelaLib;
using System.Diagnostics;

namespace VelaLib
{
    public class LinuxFileService : IFileService
    {
        public async Task Chmod(string filepath, string action)
        {
            using var process = Process.Start("chmod", $"{action} \"{filepath}\"");
            await process.WaitForExitAsync();
            if (process.ExitCode != 0)
            {
                string info1 = null;
                string info2 = null;
                try
                {
                    info1 = process.StandardOutput.ReadToEnd();
                }
                catch
                {

                }
                try
                {
                    info2 = process.StandardError.ReadToEnd();
                }
                catch
                {

                }
                var errInfo = $"{info1} Chmod error:{process.ExitCode}\r\n{info2}";
                throw new Exception(errInfo);
            }
        }

        public async Task ChmodAll(string workdir, string action)
        {
            using var process = new LinuxCmdRunner().RunInBash(workdir , $"chmod -R {action} *");
            await process.WaitForExitAsync();
            if (process.ExitCode != 0)
            {
                string info1 = null;
                string info2 = null;
                try
                {
                    info1 = process.StandardOutput.ReadToEnd();
                }
                catch
                {

                }
                try
                {
                    info2 = process.StandardError.ReadToEnd();
                }
                catch
                {

                }
                var errInfo = $"{info1} Chmod error:{process.ExitCode}\r\n{info2}";
                throw new Exception(errInfo);
            }
        }
    }
}
