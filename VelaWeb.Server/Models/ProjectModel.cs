using LibGit2Sharp;
using VelaLib;
using VelaWeb.Server.Infrastructures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using VelaAgent.DBModels.Dtos;
using LibGit2Sharp.Handlers;
using System.Net.WebSockets;
using System.IO;
using Way.Lib;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using VelaWeb.Server.Git;

namespace VelaWeb.Server.Models
{
    public class ProjectModel : Project
    {
        public AgentModel OwnerServer { get; set; }
        /// <summary>
        /// 发布人
        /// </summary>
        public string User { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [JsonIgnore]
        public string Status { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [JsonIgnore]
        public string Error { get; set; }


        string _githash;
        public string GetGitHash()
        {
            if (string.IsNullOrWhiteSpace(GitUrl))
                return $"P_{this.Guid}";

            return _githash ??= Path.GetFileNameWithoutExtension(GitUrl.Trim()).ToLower() + "_" + SysUtility.GetMD5String(Encoding.UTF8.GetBytes(GitUrl.Trim().ToLower() + this.GitRemote.ToLower() + this.BranchName.ToLower()));

        }
        public string GetGitPassword()
        {
            return Way.Lib.AES.Decrypt(GitPwd, Global.SecretKey);
        }

        public async Task Restore(string backupFileName)
        {
            var projectBuildInfoOutput = Global.ServiceProvider.GetRequiredService<IProjectBuildInfoOutput>();
            using ClientWebSocket webSocket = Global.ServiceProvider.GetRequiredService<ClientWebSocket>();
            await webSocket.ConnectAsync(new Uri($"wss://{this.OwnerServer.Address}:{this.OwnerServer.Port}/Restore"), CancellationToken.None);

            try
            {
                await webSocket.SendString(this.Guid);
                await webSocket.SendString(backupFileName);
                this.Status = $"正在还原...";
                await projectBuildInfoOutput.OutputBuildInfoAsync(this, this.Status, true);

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
                        await projectBuildInfoOutput.OutputBuildInfoAsync(this, buffer.Slice(0, ret.Count).ToArray(), false, false);
                    }
                    else
                    {
                        var info = Encoding.UTF8.GetString(data, 0, ret.Count);
                        await projectBuildInfoOutput.OutputBuildInfoAsync(this, info, false);
                    }
                }

            }
            catch (NormalClosureException)
            {
                this.Status = $"还原成功";
                await projectBuildInfoOutput.OutputBuildInfoAsync(this, this.Status, false);
            }
            catch (Exception e)
            {
                this.Status = null;
                this.Error = "发生异常";
                await projectBuildInfoOutput.OutputBuildInfoAsync(this, $"还原失败，{e.Message}", false);
            }
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
        }

        public string GetPublishPath()
        {
            var folderHash = this.GetGitHash();

            var gitFolder = $"./ProjectCodes/{folderHash}";

            gitFolder = Path.GetFullPath(gitFolder, AppDomain.CurrentDomain.BaseDirectory);
            string outputDir;
            if (this.IsNeedBuild == false)
            {
                outputDir = Path.GetFullPath(this.ProgramPath, gitFolder);
            }
            else
            {
                outputDir = Path.GetFullPath(this.ProgramPath, gitFolder);
                outputDir = Path.GetFullPath(this.BuildPath, outputDir);
            }
            return outputDir;
        }

        public async Task BuildAndPublish(RequestBuilding requestBuilding, int cols, int rows)
        {
            var buildingManager = Global.ServiceProvider.GetRequiredService<BuildingManager>();
            var gitService = Global.ServiceProvider.GetRequiredService<IGitService>();
            var projectBuildInfoOutput = Global.ServiceProvider.GetRequiredService<IProjectBuildInfoOutput>();
            var webSocketConnectionCenter = Global.ServiceProvider.GetRequiredService<WebSocketConnectionCenter>();
            var projectCenter = Global.ServiceProvider.GetRequiredService<ProjectCenter>();

            TtyWorker ttyWorker = null;
            try
            {
                BuildCommandParser buildCommandParser = new BuildCommandParser(this.BuildCmd);

                var folderHash = GetGitHash();
                var gitFolder = $"./ProjectCodes/{folderHash}";

                // _OutputInfoBuffer.Clear();

                gitFolder = Path.GetFullPath(gitFolder, AppDomain.CurrentDomain.BaseDirectory);
                string outputDir;
                //if (this.IsNeedBuild == false)
                //{
                //    outputDir = Path.GetFullPath(this.ProgramPath, gitFolder);
                //}
                //else
                {
                    outputDir = Path.GetFullPath((this.ProgramPath?.Trim() == "./" || this.ProgramPath?.Trim() == ".\\") ? "" : this.ProgramPath?.Trim(), gitFolder);
                    outputDir = Path.GetFullPath((this.BuildPath?.Trim() == "./" || this.BuildPath?.Trim() == ".\\") ? "" : this.BuildPath?.Trim(), outputDir);

                    //if ( !string.IsNullOrWhiteSpace(this.GitUrl) && outputDir == gitFolder)
                    //    throw new Exception("发布文件所在文件夹不能是git仓库的根目录");

                    //if (gitService.CheckDirIsIgnored(outputDir, gitFolder) == false)
                    //{
                    //    throw new ServiceException($"编译后文件所在文件夹为{Path.Combine(this.ProgramPath, this.BuildPath)}，此文件夹不被git忽略，请重新设置一个被git忽略的目录");
                    //}

                    if (!string.IsNullOrWhiteSpace(this.BuildPath) && this.BuildPath.Trim() != "./" && this.BuildPath.Trim() != ".\\")
                    {
                        //如果BuildPath != "" && BuildPath != "./" ，则会先删除此目录下的文件
                        if (Directory.Exists(outputDir))
                        {
                            //先删除输出文件夹
                            SysUtility.DeleteFolder(outputDir);
                        }
                    }
                }

                if (Directory.Exists(outputDir) == false)
                {
                    Directory.CreateDirectory(outputDir);
                }

                var workdir = Path.GetFullPath(this.ProgramPath == "./" ? "" : this.ProgramPath, gitFolder);
                if(Directory.Exists(workdir) == false)
                {
                    Directory.CreateDirectory(workdir);
                }

                if (!string.IsNullOrWhiteSpace(this.BuildCmd))
                {
                    ttyWorker = Global.ServiceProvider.GetRequiredService<TtyWorker>();
                    buildingManager.AddWorker(this.Guid, ttyWorker);
                    ttyWorker.Received += (s, data) =>
                    {
                        return projectBuildInfoOutput.OutputBuildInfoAsync(this, data, false, true);
                    };

                    if (cols != 0 && rows != 0)
                    {
                        await ttyWorker.Init(this.Guid, cols, rows);
                    }
                    else
                    {
                        await ttyWorker.Init(this.Guid, 120, 44);
                    }

                    if (OperatingSystem.IsWindows())
                    {
                        ttyWorker.SendCommand($"cd /d \"{workdir}\"");
                    }
                    else
                    {
                        ttyWorker.SendCommand($"cd \"{workdir}\"");
                    }
                }


                string[] cmds = buildCommandParser.BeforeCommands;
                if (cmds == null)
                    cmds = new string[0];

                if(buildCommandParser.Commands != null)
                {
                    cmds = cmds.Concat(buildCommandParser.Commands).ToArray();
                }

                if (cmds.Length > 0)
                {
                    this.Status = "执行命令...";
                    await projectBuildInfoOutput.OutputBuildInfoAsync(this, "执行命令...", false);

                    await ttyWorker.SendCommands(cmds);

                    await projectBuildInfoOutput.OutputBuildInfoAsync(this, "编译命令执行完毕", false);
                }

                requestBuilding.Dispose();

                var outputId = DateTime.Now.Ticks;

                this.Status = "正在上传文件...";
                await projectBuildInfoOutput.OutputBuildInfoAsync(this, "正在计算需要上传的文件...", false);

                using var taskWorker = new TaskWorker(this.Guid);
                buildingManager.AddWorker( this.Guid, taskWorker);

                var runRet = await Global.ServiceProvider.GetRequiredService<IUploader>().Upload(this, outputDir, cols, rows, taskWorker.CancellationToken, async (path, data, percent) =>
                {
                    if (data != null)
                    {
                        this.Status = $"正在生成映像...";
                        await projectBuildInfoOutput.OutputBuildInfoAsync(this, data, false, false);
                    }
                    else
                    {
                        if (percent < 0)
                        {
                            this.Status = $"正在处理文件...";
                            await projectBuildInfoOutput.OutputBuildInfoAsync(this, path, false);
                        }
                        else
                        {
                            this.Status = $"正在上传文件{percent}...";

                            await projectBuildInfoOutput.OutputBuildInfoAsync(this, $"\x1b[2K\r正在上传{percent}% {path}", false);

                        }
                    }
                });

                taskWorker.Dispose();

                this.Status = null;

                if (buildCommandParser.AfterCommands != null && buildCommandParser.AfterCommands.Length > 0)
                {
                    await ttyWorker.SendCommands(buildCommandParser.AfterCommands);
                }

                await projectBuildInfoOutput.OutputBuildInfoAsync(this, "\r\n部署完毕", false);
                if (runRet == false)
                {
                    if (this.RunType != Project_RunTypeEnum.Program)
                    {
                        await projectBuildInfoOutput.OutputBuildInfoAsync(this, "由于是第一次运行项目，不会自动运行，需要您确认一下配置文件没问题后，手动再发布一次。", false);
                    }
                    else if (!string.IsNullOrWhiteSpace(this.RunCmd))
                    {
                        await projectBuildInfoOutput.OutputBuildInfoAsync(this, "由于是第一次运行项目，不会自动运行，需要您确认一下配置文件没问题后，手动运行一次。", false);
                    }
                }

            }
            catch (OperationCanceledException ex)
            {
                this.Status = null;
                this.Error = "操作取消";
                await projectBuildInfoOutput.OutputBuildInfoAsync(this, $"\r\n{ex.Message}", false);
            }
            catch (Exception ex)
            {
                this.Status = null;
                this.Error = "发生异常";
                if (string.IsNullOrWhiteSpace(ex.Message))
                {
                    await projectBuildInfoOutput.OutputBuildInfoAsync(this, $"发布过程发生异常", false);
                }
                else
                {
                    await projectBuildInfoOutput.OutputBuildInfoAsync(this, $"发生异常，{ex.GetMessage()}", false);
                }
            }
            finally
            {
                if (ttyWorker != null)
                {
                    buildingManager.RemoveWorker(this.Guid , ttyWorker);
                    ttyWorker.Dispose();
                }
            }
        }

    }


}
