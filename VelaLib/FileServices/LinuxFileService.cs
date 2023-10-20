using VelaLib;
using System.Diagnostics;

namespace VelaLib
{
    public class LinuxFileService : IFileService
    {
        public async Task Chmod(string filepath, string action)
        {
            var process = Process.Start("chmod", $"{action} \"{filepath}\"");
            await process.WaitForExitAsync();
            if (process.ExitCode != 0)
            {
                var errInfo = $"{process.StandardOutput.ReadToEnd()}\r\n{process.StandardError.ReadToEnd()}";
                throw new Exception(errInfo);
            }
        }
    }
}
