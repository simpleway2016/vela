using JMS.Token.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using VelaLib;
using VelaWeb.Server;
using VelaWeb.Server.DBModels;
using VelaWeb.Server.Infrastructures;
using System.Collections;
using System.Diagnostics;
using System.Text.Json;
using Way.Lib;
using VelaWeb.Server.Models;
using System.Net.WebSockets;
using VelaWeb.Server.AutoRun;
using LibGit2Sharp;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions;
using JMS.FileUploader.AspNetCore;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using System.Text;
using VelaLib.Dtos;
using VelaLib.Windows;
using VelaLib.Linux;
using VelaWeb.Server.Git;
using Vela.CodeParser.CSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using VelaWeb.Server.CodeParsers;
using System.IO.Compression;

namespace VelaWeb
{
    public class NormalPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) => name;
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            ThreadPool.SetMinThreads(500, 500);

            var builder = WebApplication.CreateBuilder(args);

            if (!string.IsNullOrWhiteSpace(builder.Configuration["Https:Cert"]))
            {
                var uri = new Uri(builder.Configuration["Urls"].Replace("*", "a.com"));
                builder.WebHost.ConfigureKestrel(options =>
                {
                    // 禁用 HTTP/2.0 ，用2.0浏览器在重用连接时，竟然用 CONNECT 方法来连接websocket，连接websocket会提示405错误，无法连接
                    options.ConfigureEndpointDefaults(endpointOptions =>
                    {
                        endpointOptions.Protocols = HttpProtocols.Http1;
                    });

                    options.Listen(IPAddress.Any, uri.Port, listenOptions =>
                    {
                        var serverCertificate = new X509Certificate2(builder.Configuration["Https:Cert"], builder.Configuration["Https:Password"]);
                        var httpsConnectionAdapterOptions = new HttpsConnectionAdapterOptions()
                        {
                            ServerCertificate = serverCertificate,

                        };

                        var sslProtocols = builder.Configuration.GetSection("Https:SslProtocols").Get<SslProtocols?>();
                        if(sslProtocols != null)
                        {
                            httpsConnectionAdapterOptions.SslProtocols = sslProtocols.Value;
                        }
                        listenOptions.UseHttps(httpsConnectionAdapterOptions);
                    });
                });
            }

            var services = builder.Services;

            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });
            services.AddResponseCompression(options =>
            {
                //options.EnableForHttps = true;
                // 添加br与gzip的Provider
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                // 扩展一些类型 (MimeTypes中有一些基本的类型,可以打断点看看)
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    "text/html; charset=utf-8",
                    "application/xhtml+xml",
                    "application/atom+xml",
                    "image/svg+xml"
                });
            });

            services.AddSingleton<IGitService, DefaultGitService>();
            if (OperatingSystem.IsWindows())
            {
                services.AddSingleton<ICmdRunner, WindowsCmdRunner>();
                services.AddSingleton<IFileService, WindowsFileService>();
                services.AddTransient<ITerminal, WindowsTerminal>();
            }
            else
            {
                services.AddSingleton<ICmdRunner, LinuxCmdRunner>();
                services.AddSingleton<IFileService, LinuxFileService>();

                if (OperatingSystem.IsMacCatalyst() || OperatingSystem.IsMacOS())
                {
                    services.AddTransient<ITerminal, MacTerminal>();
                }
                else
                {
                    services.AddTransient<ITerminal, LinuxTerminal>();
                }
            }

            //注册代码解析器
            services.RegisterCodeParser<CSharpCodeParser>();
            services.RegisterCodeParser<JsonCodeParser>();

            services.AddTransient<TtyWorker>();
            services.AddSingleton<GitManager>();
            services.AddSingleton<AlarmManager>();
            services.AddSingleton<AgentsManager>();
            services.AddSingleton<IUploader, HttpPostUploader>();
            services.AddSingleton<WebSocketConnectionCenter>();
            services.AddSingleton<IProjectBuildInfoOutput, WebSocketAndLoggingProjectStateOutput>();
            services.AddSingleton<ProjectCenter>();
            services.AddSingleton<IUpgradePackageService, DefaultUpgradePackageService>();
            services.AddSingleton<DeleteLogs>();
            services.AddSingleton<BuildingManager>();
            services.AddTransient<JMS.JmsUploadClient>();

            services.AddMvc(option =>
            {
                option.Filters.Add<OutputResultFilter>();
            });
            services.AddControllers().AddJsonOptions(c =>
            {
                c.JsonSerializerOptions.PropertyNamingPolicy = new NormalPolicy();
            });
            //关闭参数自动校验
            services.Configure<ApiBehaviorOptions>((o) =>
            {
                o.SuppressModelStateInvalidFilter = true;
            });

            services.AddCors(options =>
            {
                options.AddPolicy("abc", builder =>
                {
                    //App:CorsOrigins in appsettings.json can contain more than one address with splitted by comma.
                    builder
                      .SetIsOriginAllowed(_ => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();

                });
            });

            services.AddSingleton<ICertificateService, DefaultCertificateService>();
            services.AddHttpClient("").ConfigurePrimaryHttpMessageHandler(() =>
            {
                // 创建HttpClientHandler并设置客户端证书
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, error) => true;
                handler.ClientCertificates.Add(Global.ClientCert);
                return handler;
            });

            services.AddTransient<ClientWebSocket>(s =>
            {
                ClientWebSocket webSocket = new ClientWebSocket();
                webSocket.Options.ClientCertificates.Add(Global.ClientCert);
                webSocket.Options.RemoteCertificateValidationCallback = (message, cert, chain, error) => true;
                return webSocket;
            });


            services.AddJmsTokenAspNetCore(null, new string[] { "Authorization" });

            services.AddScoped<SysDBContext>();
            // Add services to the container.

            builder.Services.AddControllers();

            var app = builder.Build();
            app.UseResponseCompression();//开启gzip
            app.UseCors("abc");

            Global.Init(app.Services);


            app.Services.GetRequiredService<DeleteLogs>().Start();
            app.Services.GetRequiredService<AgentsManager>().Init();
            app.Services.GetRequiredService<ProjectCenter>().Init();
            app.Services.GetRequiredService<AlarmManager>().Init();

            // Configure the HTTP request pipeline.

            app.UseJmsFileUploader();

            app.UseStaticFiles();
            //app.UseFileServer();

            app.UseWebSockets();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();


            using (var db = new SysDBContext())
            {
                if (db.FileDeleteSetting.Count() == 0)
                {
                    var exts = new string[] { ".zip", ".rar" };
                    foreach (var ext in exts)
                    {
                        db.Insert(new FileDeleteSetting
                        {
                            Ext = ext,
                            Days = 5
                        });
                    }
                }

            }

            app.Run();
        }

    }
}