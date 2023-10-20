using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VelaLib
{
    public delegate Task DataReceivedHandler(object sender, byte[] data);
    public interface ITerminal : IDisposable
    {
        bool SubProcessExited { get; }

        void Run(string command, int cols, int rows);
        void SendCommand(string command);
        void SendKill();

        Task<int> ReadAsync(byte[] data, int offset, int count);
    }
}
