using System.Diagnostics;
using System.IO.Compression;
using System.Text;

namespace VelaService
{
    internal class Program
    {
        static Process _process;
        static ServiceConfigModel _config;
        static ProcessStartInfo _processStartInfo;
        static void Main(string[] args)
        {
            typeof(ServiceConfigModel).GetMembers();
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            if (args == null || args.Length == 0)
            {
                if (File.Exists("./VelaServiceConfig.json") == false)
                {
                    throw new Exception($"没有找到{Path.GetFullPath("VelaServiceConfig.json")}文件");
                }

                var modelJson = File.ReadAllText("./VelaServiceConfig.json", Encoding.UTF8);
                _config = System.Text.Json.JsonSerializer.Deserialize<ServiceConfigModel>(modelJson);

                if (OperatingSystem.IsWindows())
                {
                    windowsSetup();
                }
                else
                {
                    linuxSetup();
                }
            }
            else if(args.Contains("-exec"))
            {
                if (File.Exists("./VelaServiceConfig.setup.json"))
                {
                    var modelJson = File.ReadAllText("./VelaServiceConfig.setup.json", Encoding.UTF8);
                    _config = System.Text.Json.JsonSerializer.Deserialize<ServiceConfigModel>(modelJson);
                }
                else
                {
                    if (File.Exists("./VelaServiceConfig.json") == false)
                    {
                        throw new Exception($"没有找到{Path.GetFullPath("VelaServiceConfig.json")}文件");
                    }

                    var modelJson = File.ReadAllText("./VelaServiceConfig.json", Encoding.UTF8);
                    _config = System.Text.Json.JsonSerializer.Deserialize<ServiceConfigModel>(modelJson);
                }

                AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

                string filename;
                string argString = null;
                if (_config.ExecStart.StartsWith("\""))
                {
                    filename = _config.ExecStart.Substring(1);
                    filename = filename.Substring(0, filename.IndexOf("\""));
                    argString = _config.ExecStart.Substring(filename.Length + 2).Trim();
                }
                else
                {
                    filename = _config.ExecStart.Split(' ')[0];
                    if (_config.ExecStart.Length > filename.Length)
                    {
                        argString = _config.ExecStart.Substring(filename.Length).Trim();
                    }
                }
                Chmod(filename, "+x");

                _processStartInfo = new ProcessStartInfo(filename, argString);

                _processStartInfo.WorkingDirectory = _config.WorkDir;

                Console.WriteLine($"工作目录{_processStartInfo.WorkingDirectory}");

                runSubProcess();

                while (true)
                {
                    Thread.Sleep(10000000);
                }
            }
        }

        static void windowsSetup()
        {
            throw new PlatformNotSupportedException();
        }

        static void linuxSetup()
        {
            Console.WriteLine("您要用哪个用户运行这个服务(直接回车则默认使用root):");
            var username = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(username))
                username = "root";

            var cmd = new LinuxCmdRunner();

           

            var optFolder = "/opt/software"; //Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var serviceFolder = Path.Combine(optFolder, _config.ServiceName);
            var workFolder = Path.Combine(optFolder, _config.ServiceName + "-application");

            if (Directory.Exists(serviceFolder) == false)
            {
                Directory.CreateDirectory(serviceFolder);
            }
            if (Directory.Exists(workFolder) == false)
            {
                Directory.CreateDirectory(workFolder);
            }

            if (username != "root")
            {
                //创建用户
                try
                {
                    cmd.RunForResult(null, "useradd -m " + username);
                }
                catch (Exception)
                {
                    try
                    {
                        //创建家目录
                        if(Directory.Exists($"/home/{username}") == false)
                        {
                            cmd.RunForResult(null, $"mkdir /home/{username}");
                            cmd.RunForResult(null, $"chown -R {username} /home/{username}");
                            cmd.RunForResult(null, $"chmod -R 700 /home/{username}");
                        }
                       
                    }
                    catch (Exception)
                    {
                         
                    }
                }
            }

            new LinuxSystemService().Register(username, _config.ServiceName, _config.Description, serviceFolder, Path.Combine(serviceFolder, "VelaService") + " -exec");

            IOUtility.CopyFolder(AppDomain.CurrentDomain.BaseDirectory, serviceFolder);
            IOUtility.CopyFolder(AppDomain.CurrentDomain.BaseDirectory, workFolder);

            //更改所有者
            StringBuilder cmds = new StringBuilder();
            cmds.AppendLine($"chown -R {username} \"{serviceFolder}\"");
            cmds.AppendLine($"chmod -R 700 \"{serviceFolder}\"");
            cmds.AppendLine($"chown -R {username} \"{workFolder}\"");
            cmds.AppendLine($"chmod -R 700 \"{workFolder}\"");

            if(username != "root" && _config.AddUserToDockerGroup)
            {
                cmds.AppendLine($"groupadd -f docker");
                cmds.AppendLine($"usermod -aG docker {username}");

                Console.WriteLine($"add {username} to docker group");
            }

            var p = cmd.RunInBash(null, cmds.ToString());
            p.WaitForExit();
            Console.Write( p.StandardOutput.ReadToEnd() + p.StandardError.ReadToEnd());
            if (p.ExitCode != 0)
                return;
                                  

            _config.WorkDir = workFolder;
            if (_config.ExecStart.StartsWith("./"))
            {
                _config.ExecStart = _config.WorkDir + _config.ExecStart.Substring(1);
            }
            else if (_config.ExecStart.StartsWith("\"./"))
            {
                _config.ExecStart = "\"" + _config.WorkDir + _config.ExecStart.Substring(2);
            }

            File.WriteAllText(serviceFolder + "/VelaServiceConfig.setup.json", System.Text.Json.JsonSerializer.Serialize(_config), Encoding.UTF8);

            Console.WriteLine($"程序安装到：{workFolder}");
            Console.WriteLine($"服务安装到：{serviceFolder}");
            Console.WriteLine($"请用 systemctl start {_config.ServiceName} 启动服务");
        }

        static void runSubProcess()
        {
            _process = Process.Start(_processStartInfo);
            Console.WriteLine($"进程启动，id{_process.Id}");

            if (_process.WaitForExit(10000))
            {
                Console.WriteLine($"进程{_process.Id}短时间内结束，不再自动重启");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                return;
            }
            _process.EnableRaisingEvents = true;
            _process.Exited += _process_Exited;            
        }

        private static void _process_Exited(object? sender, EventArgs e)
        {
            _process.Dispose();

            var upgradeConfigFile = "VelaService.upgrade.json";
            upgradeConfigFile = Path.Combine(_config.WorkDir, upgradeConfigFile);
            if (File.Exists(upgradeConfigFile))
            {
                Console.WriteLine($"发现{upgradeConfigFile}");

                var modelJson = File.ReadAllText(upgradeConfigFile, Encoding.UTF8);
                var model = System.Text.Json.JsonSerializer.Deserialize<ServiceUpgradeConfigModel>(modelJson);

                if (model.Zip.StartsWith("./"))
                {
                    model.Zip = Path.Combine(_config.WorkDir, model.Zip.Substring(2));
                }
                else
                {
                    model.Zip = Path.Combine(_config.WorkDir, model.Zip);
                }
                Console.WriteLine($"准备解压{model.Zip}");

                using (ZipArchive zipArchive = ZipFile.OpenRead(model.Zip))
                {
                    foreach (ZipArchiveEntry entry in zipArchive.Entries)
                    {
                        try
                        {
                            if (model.ExcludeFiles != null && model.ExcludeFiles.Any(m=>string.Equals(m, entry.FullName, StringComparison.OrdinalIgnoreCase)))
                            {
                                Console.WriteLine($"忽略{entry.FullName}");
                                continue;
                            }
                               

                            string filePath = Path.Combine(_config.WorkDir, entry.FullName);
                            var dir = Path.GetDirectoryName(filePath);
                            if (Directory.Exists(dir) == false)
                            {
                                Directory.CreateDirectory(dir);
                            }

                            entry.ExtractToFile(filePath, true);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }

                Console.WriteLine($"解压完毕");

                try
                {
                    IOUtility.DeleteFile(upgradeConfigFile);
                    IOUtility.DeleteFile(model.Zip);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            runSubProcess();
        }

        private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
            Console.WriteLine("ready to kill application");
            if (_process != null)
            {              

                if (_config.Kill == -9)
                {
                    _process.Kill();
                }
                else
                {
                    Process.Start("kill", $"{_config.Kill} {_process.Id}");
                    if (_process.WaitForExit(5000) == false)
                    {
                        _process.Kill();
                    }

                }
                _process.Dispose();
                _process = null;

            }
        }

        static void Chmod(string filepath, string action)
        {
            using var process = Process.Start("chmod", $"{action} \"{filepath}\"");
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                string info1 = null;
                string info2 = null;
                try
                {
                    info1 = process.StandardOutput.ReadToEnd();
                }
                catch
                {

                }
                try
                {
                    info2 = process.StandardError.ReadToEnd();
                }
                catch
                {

                }
                var errInfo = $"{info1}\r\n{info2}";
                throw new Exception(errInfo);
            }
        }
    }
}