using System.Diagnostics;
using VelaAgent.DBModels;
using VelaAgent.Infrastructures;
using VelaLib;

namespace VelaAgent.ProgramOutput
{
    public class ProgramOutput : IProgramOutput
    {
        ICmdRunner _cmdRunner;
        Process _process;
        Process _process_err;
        public ProgramOutput()
        {
            _cmdRunner = Global.ServiceProvider.GetRequiredService<ICmdRunner>();
        }

        public void Dispose()
        {
            
            try
            {
                _process_err?.Dispose();
            }
            catch
            {

            }

            var p = _process;
            try
            {
                p?.Kill();
            }
            catch
            {

            }
        }

        public async Task StartOutput(Project project, IInfoOutput infoOutput, int preLines)
        {
            if (string.IsNullOrEmpty(project.LogPath))
            {
                await infoOutput.Output($"没有设置日志文件路径");
                return;
            }

            var logpath = Path.Combine(Global.AppConfig.Current.PublishRootPath, project.Name, project.LogPath);

            if (File.Exists(logpath) == false)
            {
                await infoOutput.Output($"找不到日志文件{logpath}");
                return;
            }
            _process = _cmdRunner.Run(null, $"tail -n {preLines} -f \"{logpath}\"");

            if (File.Exists($"{logpath}.err"))
            {
                outputError(project, infoOutput, preLines);
            }

            string line;
            while (true)
            {
                line = await _process.StandardOutput.ReadLineAsync();
                if (line == null)
                {
                    break;
                }

                await infoOutput.Output(line);
            }
            await _process.WaitForExitAsync();


            _process.Dispose();
            _process = null;
        }

        async void outputError(Project project, IInfoOutput infoOutput, int preLines)
        {
            try
            {
                var logpath = Path.Combine(Global.AppConfig.Current.PublishRootPath, project.Name, project.LogPath);
                _process_err = _cmdRunner.Run(null, $"tail -n {preLines} -f \"{logpath}.err\"");

                string line;
                while (true)
                {
                    line = await _process_err.StandardOutput.ReadLineAsync();
                    if (line == null)
                    {
                        break;
                    }

                    await infoOutput.Output(line);
                }
            }
            catch
            {

            }
        }
    }
}
