using VelaLib;
using System.Diagnostics;
using System.Text;

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
                var errInfo = $"{info1}\r\n{info2}";
                throw new Exception(errInfo);
            }
        }

        public async Task ChmodAll(string workdir, string action)
        {
            if (workdir == null)
                workdir = "./";

            StringBuilder errs = new StringBuilder();
            await ChmodFolderAll(workdir, action, errs);
            if (errs.Length > 0)
            {
                var msg = errs.ToString();
                errs.Clear();
                throw new Exception(msg);
            }
        }

        async Task ChmodFolderAll(string folder, string action, StringBuilder errs)
        {
           
            var subFolders = Directory.GetDirectories(folder);
            foreach (var subFolder in subFolders) {
                await ChmodFolderAll(subFolder, action, errs);
            }

            var files = Directory.GetFiles(folder);
            foreach (var file in files) {
                try
                {
                    await Chmod(file, action);
                }
                catch (Exception ex)
                {
                    errs.AppendLine($"chmod {action} {file} 失败");
                }
            }
        }
    }
}
