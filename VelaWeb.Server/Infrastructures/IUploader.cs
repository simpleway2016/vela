using VelaLib;
using VelaLib.Dtos;
using VelaWeb.Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Way.Lib;
using HttpClient = System.Net.Http.HttpClient;
using System.Net.WebSockets;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VelaWeb.Server.Infrastructures
{
    public interface IUploader
    {
        Task<bool> Upload(ProjectModel projectModel, string sourceFolder, int cols, int rows,CancellationToken cancellationToken, Func<string, byte[], int, Task> progressHandler);
    }

    public class HttpPostUploader : IUploader
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpPostUploader(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<bool> Upload(ProjectModel projectModel, string sourceFolder, int cols, int rows,CancellationToken cancellationToken, Func<string, byte[], int, Task> progressHandler)
        {

            //先检查需要提交哪些文件
            var client = _httpClientFactory.CreateClient("");

            var files = SysUtility.GetFolderFiles(sourceFolder, sourceFolder);
            if (files.Length == 0)
                throw new Exception("在编译输出目录没有找到任何文件");

            var fileInfos = files.Select(m => new UploadFileInfo()
            {
                Path = m,
                Length = new FileInfo(Path.Combine(sourceFolder, m)).Length,
            }).ToArray();

            await Parallel.ForEachAsync(fileInfos, async (item, token) =>
            {
                item.MD5 = await SysUtility.CalculateMD5Async(Path.Combine(sourceFolder, item.Path));
            });

            HttpContent content = new StringContent(fileInfos.ToJsonString());
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            using var response = await client.PostAsync($"https://{projectModel.OwnerServer.Address}:{projectModel.OwnerServer.Port}/Publish/GetNeedUploadFiles?projectGuid={projectModel.Guid}", content);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                var msg = await response.Content.ReadAsStringAsync();
                throw new Exception($"GetNeedUploadFiles执行失败,{msg}");
            }

            await progressHandler("开始上传", null, 0);

            var uploadPaths = (await response.Content.ReadAsStringAsync()).FromJson<string[]>();
            int done = 0;
            DateTime startTime = DateTime.Now;
            foreach (var filepath in uploadPaths)
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException("操作取消");

                if ((DateTime.Now - startTime).TotalSeconds >= 2)
                {
                    startTime = DateTime.Now;
                    await progressHandler(filepath, null, done * 100 / uploadPaths.Length);
                }
                await UploadFile(client, projectModel, filepath, $"https://{projectModel.OwnerServer.Address}:{projectModel.OwnerServer.Port}/Publish/Upload", Path.Combine(sourceFolder, filepath) , cancellationToken);
                done++;
            }
            await progressHandler("文件传输完毕\r\n", null, 100);

            return await UploadCompleted(client, projectModel, fileInfos, cols, rows, progressHandler);
        }

        async Task<bool> UploadCompleted(HttpClient client, ProjectModel localProjectItem, UploadFileInfo[] fileInfos, int cols, int rows, Func<string, byte[], int, Task> progressHandler)
        {
            Global.ServiceProvider.GetRequiredService<ProjectCenter>().SetProjectStartTime(localProjectItem.Guid, DateTime.Now);

            using ClientWebSocket webSocket = Global.ServiceProvider.GetRequiredService<ClientWebSocket>();
            await webSocket.ConnectAsync(new Uri($"wss://{localProjectItem.OwnerServer.Address}:{localProjectItem.OwnerServer.Port}/UploadCompleted?cols={cols}&rows={rows}"), CancellationToken.None);

            try
            {
                await webSocket.SendString(fileInfos.Select(m => m.Path).ToJsonString());
                await webSocket.SendString(localProjectItem.Guid);

                var data = new byte[4096];
                var buffer = new ArraySegment<byte>(data);
                while (true)
                {
                    var ret = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                    if (ret.CloseStatus != null)
                    {
                        if (ret.CloseStatus == WebSocketCloseStatus.NormalClosure)
                        {
                            throw new NormalClosureException(ret.CloseStatusDescription);
                        }
                        else
                        {
                            throw new WebSocketException(ret.CloseStatusDescription);

                        }
                    }

                    if (ret.MessageType == WebSocketMessageType.Binary)
                    {

                        await progressHandler.Invoke(null, buffer.Slice(0, ret.Count).ToArray(), -1);
                    }
                    else
                    {
                        var info = Encoding.UTF8.GetString(data, 0, ret.Count);
                        await progressHandler.Invoke(info, null, -1);
                    }
                }
            }
            catch (NormalClosureException ex)
            {
                return ex.Message == "1";
            }
            finally
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
            }
        }

        async Task UploadFile(HttpClient httpClient, ProjectModel localProjectItem, string path, string url, string filePath,CancellationToken cancellationToken)
        {
            using (var formContent = new MultipartFormDataContent())
            {
                // 读取文件内容
                byte[] fileBytes = SysUtility.Compress(File.ReadAllBytes(filePath));
                var fileinfo = new FileInfo(filePath);

                // 创建文件内容的HttpContent
                var fileContent = new ByteArrayContent(fileBytes);
                formContent.Add(fileContent, "file", "a");

                formContent.Add(new StringContent(localProjectItem.Guid), "projectGuid");
                formContent.Add(new StringContent(path), "path");
                formContent.Add(new StringContent(await SysUtility.CalculateMD5Async(filePath)), "md5");
                formContent.Add(new StringContent(fileinfo.Length.ToString()), "length");
                formContent.Add(new StringContent(fileinfo.LastWriteTimeUtc.ToString("yyyy-MM-dd HH:mm:ss")), "writeTimeUtc");

                // 发送POST请求
                using var response = await httpClient.PostAsync(url, formContent);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    var msg = await response.Content.ReadAsStringAsync(cancellationToken);
                    throw new Exception(msg);
                }
            }
        }
    }
}
