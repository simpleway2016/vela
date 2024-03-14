using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using VelaAgent.ActionFilters;
using VelaAgent.DBModels;
using VelaAgent.Infrastructures;
using VelaLib;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using VelaAgent.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions;
using JMS.FileUploader.AspNetCore;
using VelaAgent.ProgramOutput;
using VelaAgent.AutoRun;
using VelaLib.Dtos;
using VelaLib.Windows;
using VelaLib.Linux;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VelaAgent.KeepAlive;
using System.Text.RegularExpressions;
using System;

namespace VelaAgent
{
    public class NormalPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) => name;
    }
    public class Program
    {
        public static int ProcessUserId;
        public static void Main(string[] args)
        {
            if (!OperatingSystem.IsWindows())
            {

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "id",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };


                var process = System.Diagnostics.Process.Start(startInfo);
                process.WaitForExit();
                var t1 = process.StandardOutput.ReadToEnd();
                var m = Regex.Match(t1, "uid=(?<n>[0-9]+)");
                ProcessUserId = int.Parse(m.Groups["n"].Value);
                Console.WriteLine($"Running userid:{ProcessUserId}");
            }
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            ThreadPool.SetMinThreads(300, 300);

            string pfxFilePath = "./server.pfx";
            if (File.Exists(pfxFilePath) == false)
            {
                new DefaultCertificateService().CreateCertificate("VelaAgent", 100, pfxFilePath, "123456");
            }
            var builder = WebApplication.CreateBuilder(args);
            Global.Configuration = builder.Configuration;

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Listen(IPAddress.Any, builder.Configuration.GetSection("Port").Get<int>(), listenOptions =>
                {
                    var serverCertificate = new X509Certificate2(pfxFilePath, "123456");
                    var httpsConnectionAdapterOptions = new HttpsConnectionAdapterOptions()
                    {
                        ClientCertificateMode = ClientCertificateMode.RequireCertificate,
                        SslProtocols = SslProtocols.None,
                        ClientCertificateValidation = (cer, chain, error) =>
                        {
#if DEBUG
                            return true;
#endif
                            if (!string.IsNullOrEmpty(Global.ClientCertHash) && cer.GetCertHashString() != Global.ClientCertHash)
                                return false;
                            return true;
                        },
                        ServerCertificate = serverCertificate

                    };
                    listenOptions.UseHttps(httpsConnectionAdapterOptions);
                });
            });

            var services = builder.Services;
            // Add services to the container.
            services.AddMvc(option =>
            {
                option.Filters.Add<OutputResultFilter>();
            });
            services.AddControllers();
            services.AddScoped<SysDBContext>();
            services.AddSingleton<ICertificateService, DefaultCertificateService>();
            if (OperatingSystem.IsWindows())
            {
                services.AddSingleton<ICmdRunner, WindowsCmdRunner>();
                services.AddTransient<ITerminal, WindowsTerminal>();
            }
            else
            {
                services.AddSingleton<ICmdRunner, LinuxCmdRunner>();
                if (OperatingSystem.IsMacCatalyst() || OperatingSystem.IsMacOS())
                {
                    services.AddTransient<ITerminal, MacTerminal>();
                }
                else
                {
                    services.AddTransient<ITerminal, LinuxTerminal>();
                }
            }
            services.AddTransient<TtyWorker>();
            services.AddSingleton<KeepProcessAliveFactory>();
            services.AddSingleton<IDockerEngine, CmdDockerEngine>();
            services.AddSingleton<ProjectBackup>();
            services.AddSingleton<ProgramOutputFactory>();
            services.AddSingleton<ProjectRunnerFactory>();
            services.AddSingleton<DeleteBackups>();

            //add IRunningInfoProvider
            var allTypes = typeof(IRunningInfoProvider).Assembly.GetTypes();
            var runningInfoProviderTypes = allTypes.Where(m => m.GetInterfaces().Contains(typeof(IRunningInfoProvider))).ToArray();
            services.TryAddEnumerable(runningInfoProviderTypes.Select(m => ServiceDescriptor.Singleton(typeof(IRunningInfoProvider), m)));

            services.AddControllers().AddJsonOptions(c =>
            {
                c.JsonSerializerOptions.PropertyNamingPolicy = new NormalPolicy();
            });

            if (OperatingSystem.IsWindows())
            {
                services.AddSingleton<IFileService, WindowsFileService>();
                services.AddSingleton<IProcessService, WindowsProcessService>();
            }
            else
            {
                services.AddSingleton<IFileService, LinuxFileService>();
                services.AddSingleton<IProcessService, LinuxProcessService>();
            }

            //关闭参数自动校验
            services.Configure<ApiBehaviorOptions>((o) =>
            {
                o.SuppressModelStateInvalidFilter = true;
            });

            var app = builder.Build();
            Global.ServiceProvider = app.Services;

            var logger = app.Services.GetService<ILogger<Program>>();

            app.Services.GetRequiredService<KeepProcessAliveFactory>().Init();
            app.Services.GetRequiredService<DeleteBackups>().Run();

            app.UseJmsFileUploader();
            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseWebSockets();

            app.MapControllers();


            app.Services.GetService<ILogger<Program>>().LogInformation($"Current Version:{typeof(PublishController).Assembly.GetName().Version.ToString()}");


            app.Run();
        }
    }
}