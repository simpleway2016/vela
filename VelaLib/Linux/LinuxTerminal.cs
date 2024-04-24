using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VelaLib.Linux
{
    public unsafe class LinuxTerminal : ITerminal
    {
        #region NATIVE

        [DllImport("./libtty.so")]
        static extern int myforkPty(int* master, int* fd_in, int* fd_out, int cols,int rows);
        [DllImport("./libtty.so")]
        static extern void closeFd(int fd);

        [DllImport("./libtty.so")]
        static extern int myWaitPid(int pid, int* stat);

        #endregion

        FileStream inputStream;
        FileStream outputStream;
        int _fd_in;
        int _fd_out;
        int _master;
        int _pid;

        public bool SubProcessExited { get; private set; }

        public void Dispose()
        {
            if(inputStream != null)
            {
                try
                {
                    inputStream.Dispose();
                }
                catch 
                {
 
                }
                inputStream = null;
            }

            if (outputStream != null)
            {
                try
                {
                    outputStream.Dispose();
                }
                catch
                {

                }
                outputStream = null;
            }

            closeFd(_fd_in);
            closeFd(_fd_out);

            if (_pid > 0)
            {
                KillProcess();
                _pid = 0;
            }
        }

        public  void Run(string command, int cols, int rows)
        {
            int master;
            int f1, f2;
            var pid = myforkPty(&master, &f1, &f2, cols,rows);

            if (pid == 0)
                return;
            _fd_in = f1; 
            _fd_out = f2;   
            _pid = pid;

            inputStream = new FileStream(new SafeFileHandle((IntPtr)_fd_in, true), FileAccess.Write);
            outputStream = new FileStream(new SafeFileHandle((IntPtr)_fd_out, true), FileAccess.Read);

            new Thread(() => {
                int state;
                for(int i = 0; i < 100; i ++)
                {
                    //必须调用这个myWaitPid等待子进程结束，否则，是kill不了子进程的
                    var p = myWaitPid(pid, &state);
                    if (p == pid)
                    {
                        break;
                    }
                }
                this.SubProcessExited = true;
            }).Start();
        }

        public Task<int> ReadAsync(byte[] data,int offset,int count)
        {
            return outputStream.ReadAsync(data, offset, count);
        }


        public void SendCommand(string command)
        {
            lock (inputStream)
            {
                inputStream.Write(Encoding.UTF8.GetBytes(command));
                inputStream.WriteByte(13);
                inputStream.Flush();
            }
        }

        public void SendKill()
        {
            lock (inputStream)
            {
                inputStream.WriteByte(0x03);
                inputStream.Flush();
            }
        }

        public void KillProcess()
        {
            try
            {
                System.Diagnostics.Process.GetProcessById(_pid)?.Kill();
            }
            catch
            {

            }
        }
    }
}
