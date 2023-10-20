using Microsoft.Win32.SafeHandles;
using Org.BouncyCastle.Utilities.Zlib;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VelaLib.Windows
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class WindowsTerminal : ITerminal
    {
        private PseudoConsolePipe inputPipe;
        private PseudoConsolePipe outputPipe;
        private PseudoConsole pseudoConsole;
        private Process process;
        private FileStream writer;
        private FileStream reader;
        System.Diagnostics.Process _process;

        public bool SubProcessExited => _process == null ? true : _process.HasExited;

        public WindowsTerminal()
        {

        }


        /// <summary>
        /// Start the psuedoconsole and run the process as shown in 
        /// https://docs.microsoft.com/en-us/windows/console/creating-a-pseudoconsole-session#creating-the-pseudoconsole
        /// </summary>
        public void Run(string command, int cols, int rows)
        {
            inputPipe = new PseudoConsolePipe();
            outputPipe = new PseudoConsolePipe();
            pseudoConsole = PseudoConsole.Create(inputPipe.ReadSide, outputPipe.WriteSide, cols, rows);
            process = ProcessFactory.Start(command, PseudoConsole.PseudoConsoleThreadAttribute, pseudoConsole.Handle);
            writer = new FileStream(inputPipe.WriteSide, FileAccess.Write);
            reader = new FileStream(outputPipe.ReadSide, FileAccess.Read);

            _process = System.Diagnostics.Process.GetProcessById(process.ProcessInfo.dwProcessId);
        }

        public Task<int> ReadAsync(byte[] data, int offset, int count)
        {
            return reader.ReadAsync(data, offset, count);
        }

        public void SendCommand(string command)
        {
            writer.Write(Encoding.UTF8.GetBytes(command));
            writer.WriteByte(13);
            writer.Flush();
        }

        public void SendKill()
        {
            writer.WriteByte(0x03);
            writer.Flush();
        }

        private void DisposeResources(params IDisposable[] disposables)
        {
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }
        }

        public void Dispose()
        {           
            DisposeResources(reader, writer, process, pseudoConsole, outputPipe, inputPipe);
            KillProcess();
        }

        public void KillProcess()
        {
            try
            {
                System.Diagnostics.Process.GetProcessById(process.ProcessInfo.dwProcessId)?.Kill();
            }
            catch
            {
            }
        }
    }
}
