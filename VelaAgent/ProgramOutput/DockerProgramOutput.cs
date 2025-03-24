using Newtonsoft.Json.Linq;
using System.Diagnostics;
using VelaAgent.DBModels;
using VelaAgent.Infrastructures;
using VelaLib;

namespace VelaAgent.ProgramOutput
{
    public class DockerProgramOutput : IProgramOutput
    {
        private readonly string _containerId;
        ICmdRunner _cmdRunner;
        Process _process;
        bool _errorOutputExited = false;
        public DockerProgramOutput(string containerId) {
            _cmdRunner = Global.ServiceProvider.GetRequiredService<ICmdRunner>();
            _containerId = containerId;
        }

        public void Dispose()
        {
            var p = _process;
            try
            {
                p?.Kill();
            }
            catch 
            {

            }
        }

        public async Task StartOutput(Project project, IInfoOutput infoOutput,int preLines)
        {
            _process = _cmdRunner.Run(null, $"docker logs -f {_containerId} --tail {preLines}");

            outputError(_process.StandardError.BaseStream , infoOutput);

            var pipeReader = System.IO.Pipelines.PipeReader.Create(_process.StandardOutput.BaseStream, new System.IO.Pipelines.StreamPipeReaderOptions(null, -1, -1, true));
            string line;
            while (true)
            {
                line = await pipeReader.ReadLineAsync();
                if(line == null)
                {
                    break;
                }

                await infoOutput.Output(line);
            }
            await _process.WaitForExitAsync();
            while (!_errorOutputExited)
                await Task.Delay(1000);

            _process.Dispose();
            _process = null;
        }

        async void outputError( Stream errorStream , IInfoOutput infoOutput)
        {
            var pipeReader = System.IO.Pipelines.PipeReader.Create(errorStream, new System.IO.Pipelines.StreamPipeReaderOptions(null, -1, -1, true));
            string line;
            while (true)
            {
                line = await pipeReader.ReadLineAsync();
                if (line == null)
                {
                    break;
                }

                await infoOutput.Output($"\x1b[38;5;210m{line}\x1b[0m");
            }
            _errorOutputExited = true;
        }
    }
}
