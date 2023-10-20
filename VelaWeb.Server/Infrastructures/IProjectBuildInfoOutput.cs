using VelaLib;
using VelaWeb.Server.Models;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using Way.Lib;
using System.Net.WebSockets;

namespace VelaWeb.Server.Infrastructures
{
    public interface IProjectBuildInfoOutput
    {
        /// <summary>
        /// flush历史信息，并返回这些历史信息
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        Task<byte[]> Flush(string guid);
        Task OutputBuildInfoAsync(ProjectModel projectModel, string outputText, bool isPullStart);
        Task OutputBuildInfoAsync(ProjectModel projectModel, byte[] outputData, bool isPullStart, bool saveToFile);
    }

    public class WebSocketAndLoggingProjectStateOutput : IProjectBuildInfoOutput
    {
        private readonly WebSocketConnectionCenter _webSocketConnectionCenter;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ProjectCenter _projectCenter;
        ConcurrentDictionary<string, FileStream> _loggingFileStreams = new ConcurrentDictionary<string, FileStream>();
        static byte[] rn = Encoding.UTF8.GetBytes("\n");
        public WebSocketAndLoggingProjectStateOutput(WebSocketConnectionCenter webSocketConnectionCenter, IHttpClientFactory httpClientFactory, ProjectCenter projectCenter)
        {
            _webSocketConnectionCenter = webSocketConnectionCenter;
            _httpClientFactory = httpClientFactory;
            _projectCenter = projectCenter;
        }

        public async Task<byte[]> Flush(string guid)
        {
            if (_loggingFileStreams.TryGetValue(guid, out FileStream fileStream))
            {
                using var fs = new FileStream(fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var data = new byte[fs.Length];
                await fs.ReadAtLeastAsync(data,data.Length);
                return data;
            }
            return null;
        }

        static ConcurrentDictionary<string, JsonBody> LastJsonBody = new ConcurrentDictionary<string, JsonBody>();

        public async Task OutputBuildInfoAsync(ProjectModel projectModel, string outputText, bool isPullStart)
        {
            try
            {
                if (string.IsNullOrEmpty(outputText))
                    return;

                await _webSocketConnectionCenter.SendTextAsync(new
                {
                    Guid = projectModel.Guid,
                    Status = projectModel.Status,
                    Error = projectModel.Error
                }.ToJsonString());

                var _headDatas = Encoding.UTF8.GetBytes(projectModel.Guid);
                byte[] data;
                bool saveToFile = true;
                if (outputText.StartsWith("\x1b[2K\r"))
                {
                    saveToFile = false;
                    data = Encoding.UTF8.GetBytes($"{outputText}");
                }
                else
                {
                    data = Encoding.UTF8.GetBytes($"{outputText}\r\n");
                }
                var bs = new byte[data.Length + _headDatas.Length + 2];
                Array.Copy(_headDatas, bs, _headDatas.Length);
                Array.Copy(data, 0, bs, _headDatas.Length + 2, data.Length);

                await OutputBuildInfoAsync(projectModel, bs, isPullStart, saveToFile);
            }
            catch 
            {

            }

        }

        public async Task OutputBuildInfoAsync(ProjectModel projectModel, byte[] outputData, bool isPullStart, bool saveToFile)
        {
           await  _webSocketConnectionCenter.SendBinaryAsync(outputData);

            FileStream fileStream = null;
            if (isPullStart || _loggingFileStreams.ContainsKey(projectModel.Guid) == false)
            {
                //生成新的日志
                var folderHash = projectModel.GetGitHash();
                var logFolder = $"./Logs/{folderHash}/{projectModel.Guid}";
                if (Directory.Exists(logFolder) == false)
                {
                    Directory.CreateDirectory(logFolder);
                }

                //删除老文件
                SysUtility.DeleteOldFiles(logFolder, 7);

                var logFileName = Path.Combine(logFolder, Guid.NewGuid().ToString("N") + ".tty");
                if (_loggingFileStreams.ContainsKey(projectModel.Guid))
                {
                    fileStream = _loggingFileStreams[projectModel.Guid];
                    fileStream.Flush();
                    fileStream.Dispose();
                    LastJsonBody.TryRemove(projectModel.Guid, out _);
                }
                fileStream = _loggingFileStreams[projectModel.Guid] = new FileStream(logFileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            }

            if (fileStream == null)
            {
                _loggingFileStreams.TryGetValue(projectModel.Guid, out fileStream);
            }

            if (fileStream != null && saveToFile)
            {
                lock (fileStream)
                {
                    //写入文件时，去掉前面guid内容和后面多余的2个字节，一共34字节
                    var len = outputData.Length-34;
                    fileStream.Write(outputData,34 , len);
                    fileStream.Flush();
                }

            }
        }


    }

    class JsonBody
    {
        public long id { get; set; }
        public long __FilePosition;
        public int __LineLength;
    }
}
