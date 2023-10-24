using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Way.Lib;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VelaLib
{
    public class TtyWorker : IDisposable,IWorker
    {
        public delegate Task ReceivedHandler(object sender, byte[] data);

        private readonly ITerminal _terminal;
        private string _guid;
        private byte[] _headDatas;
        bool _sendKilled = false;
        public event ReceivedHandler Received;

        public string ProjectGuid => _guid;

        bool _disposed;
        bool _isWindows;
        static ConcurrentDictionary<TtyWorker, bool> AllWorkers = new ConcurrentDictionary<TtyWorker, bool>();

        public TtyWorker(ITerminal terminal)
        {
            _isWindows = OperatingSystem.IsWindows();
            _terminal = terminal;


        }

        public static IEnumerable<TtyWorker> FindWorkers(string projectGuid)
        {
            return (from m in AllWorkers
                    where m.Key._guid == projectGuid
                    select m.Key);
        }

        public async Task Init(string guid, int cols, int rows)
        {
            _guid = guid;
            _headDatas = Encoding.UTF8.GetBytes(guid);

            if (_isWindows)
            {
                _terminal.Run("cmd.exe", cols, rows);
            }
            else
            {
                _terminal.Run("/bin/bash", cols, rows);
            }
            AllWorkers[this] = true;

            //让终端隐藏路径
            if (_isWindows)
            {

                //SendCommand("prompt $G");//将路径变成 >
                SendCommand("@echo off"); //隐藏路径
            }
            else
            {
                SendCommand("PS1=\"\"");  //隐藏路径
                SendCommand("stty -echo");//关闭回显
                SendCommand("source /etc/profile");
            }

            //输出一下当前exitcode的值，当收到这个数值，才表示前面所有终端发送的数据接收完毕
            string printExitCodeCommand = _isWindows ? "echo vela_exitcode:%errorlevel%" : "echo vela_exitcode:$?";
            SendCommand(printExitCodeCommand);

            byte[] data = new byte[4096];
            while (!_terminal.SubProcessExited)
            {
                var len = await _terminal.ReadAsync(data, 0, data.Length);
                var text = Encoding.UTF8.GetString(data, 0, len);
                var m = Regex.Match(text, @"vela_exitcode:(?<num>[0-9\-]+)");
                if (m != null && m.Length > 0)
                {
                    break;
                }
            }


            //在确定所有数据接收完毕后，再开始接收未来的数据
            read();
        }

        async void read()
        {
            byte[] bs;
            try
            {
                byte[] buffer = new byte[4096];
                while (!_terminal.SubProcessExited)
                {
                    var len = await _terminal.ReadAsync(buffer, 0, buffer.Length);
                    var data = buffer.Take(len).ToArray();

                    if (_exitcode == null)
                    {
                        try
                        {

                            string text = Encoding.UTF8.GetString(data);
                            var m = Regex.Match(text, @"vela_exitcode:(?<num>[0-9\-]+)");
                            if (m != null && m.Length > 0)
                            {
                                _exitcode = int.Parse(m.Groups["num"].Value);
                                text = text.Replace(m.Value, "");
                                string printExitCodeCommand = OperatingSystem.IsWindows() ? "echo vela_exitcode:%errorlevel%" : "echo vela_exitcode:$?";
                                text = text.Replace(printExitCodeCommand, "");
                                data = Encoding.UTF8.GetBytes(text);
                                //continue;
                            }
                            else if (text.Contains("echo vela_exitcode:"))
                            {
                                string printExitCodeCommand = OperatingSystem.IsWindows() ? "echo vela_exitcode:%errorlevel%" : "echo vela_exitcode:$?";
                                text = text.Replace(printExitCodeCommand, "");
                                data = Encoding.UTF8.GetBytes(text);
                                // continue;
                            }
                        }
                        catch (Exception ex)
                        {
                            bs = new byte[data.Length + _headDatas.Length + 2];
                            Array.Copy(_headDatas, bs, _headDatas.Length);
                            Array.Copy(data, 0, bs, _headDatas.Length + 2, data.Length);
                            Received?.Invoke(this, bs);

                            _exitcode = 1;

                            data = Encoding.UTF8.GetBytes($"\r处理exitcode异常,{ex.ToString()}");
                            bs = new byte[data.Length + _headDatas.Length + 2];
                            Array.Copy(_headDatas, bs, _headDatas.Length);
                            Array.Copy(data, 0, bs, _headDatas.Length + 2, data.Length);
                        }
                    }

                    bs = new byte[data.Length + _headDatas.Length + 2];
                    Array.Copy(_headDatas, bs, _headDatas.Length);
                    Array.Copy(data, 0, bs, _headDatas.Length + 2, data.Length);

                    if (Received != null)
                    {
                        await Received.Invoke(this, bs);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }


        public async Task Kill()
        {
            if (_sendKilled)
                return;

            _sendKilled = true;
            //等待5秒，如果没有_disposed，主动关闭websocket
            waitToClose();

            _terminal.SendKill();
        }

        async void waitToClose()
        {
            await Task.Delay(5000);
            if (!_disposed)
            {
                this.Dispose();
            }
        }

        public void SendCommand(string text)
        {
            if (_sendKilled || _disposed)
                throw new OperationCanceledException("操作已取消");

            _terminal.SendCommand(text);
        }

        int? _exitcode = null;
        public async Task SendCommands(string[] commands)
        {
            if (_sendKilled || _disposed)
                throw new OperationCanceledException("操作已取消");

            string printExitCodeCommand = _isWindows ? "echo vela_exitcode:%errorlevel%" : "echo vela_exitcode:$?";

            foreach (var commandItem in commands)
            {
                _exitcode = null;
                bool checkExitCode = true;
                var commandtext = commandItem;
                if (commandtext.StartsWith("?"))
                {
                    checkExitCode = false;
                    //这条命令无需判断是否执行成功
                    commandtext = commandtext.Substring(1);
                }

                if (!_isWindows)
                {
                    //echo -e "\\\\"  =>最终只会输出一个斜杠
                    var text = commandtext.Replace(@"\", @"\\\\").Replace("\"", "\\\"");
                    this.SendCommand($"echo -e \"\\x1b[38;5;51m{text}\\x1b[0m\"");
                }

                this.SendCommand(commandtext);
               

                this.SendCommand(printExitCodeCommand);
                while (_exitcode == null)
                {
                    if (_sendKilled)
                    {
                        throw new OperationCanceledException("任务已取消");
                    }
                    else if (_terminal.SubProcessExited)
                    {
                        throw new Exception("程序意外关闭");
                    }
                    await Task.Delay(500);
                }

                if (_sendKilled)
                {
                    throw new OperationCanceledException("任务已取消");
                }
                else if (_terminal.SubProcessExited)
                {
                    throw new Exception("程序意外关闭");
                }

                if (_exitcode != 0 && checkExitCode)
                {
                    throw new Exception("编译错误");
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _terminal.Dispose();
                AllWorkers.TryRemove(this, out _);
            }
        }
    }
}
