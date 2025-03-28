using System.Buffers;
using System.Diagnostics;
using System.Text;
using VelaAgent.DBModels;
using VelaAgent.Infrastructures;
using VelaLib;

namespace VelaAgent.ProgramOutput
{
    public class ProgramOutput : IProgramOutput
    {
        ICmdRunner _cmdRunner;
        public ProgramOutput()
        {
            _cmdRunner = Global.ServiceProvider.GetRequiredService<ICmdRunner>();
        }
        FileStream _fs; 
        public void Dispose()
        {
            
            try
            {
                _fs?.Dispose();

            }
            catch
            {

            }

            _fs = null;
        }

        static async Task<string[]> ReadLastLinesAsync(string filePath, int linesToRead)
        {
            int offset = 10240;
            int seat = linesToRead - 1;
            string[] lines = new string[linesToRead];
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                long readedPos;//记录已经收录的行的坐标
                readedPos = fs.Length;


                while (seat >= 0 && readedPos > 0)
                {
                    var bs = ArrayPool<byte>.Shared.Rent(offset);
                    try
                    {
                        var pos = Math.Max(0, readedPos - offset);
                        fs.Position = pos;
                        var readlen = (int)(readedPos - pos);

                        long startpos = readedPos;
                        var readed = await fs.ReadAsync(bs, 0, readlen);
                        var readedLines = Encoding.UTF8.GetString(bs, 0, readed).Split('\n').Select(m => m.TrimEnd()).ToArray();

                        int fromindex = 1;
                        if (pos == 0 && readedLines.Length > 0 && readedLines[0] != null)
                        {
                            startpos = pos;
                            fromindex = 0;
                        }
                        else
                        {
                            for (int i = 0; i < readed; i++)
                            {
                                if (bs[i] == 10)
                                {
                                    startpos = i + 1 + pos;
                                    break;
                                }
                            }
                        }

                        for (int i = readedLines.Length - 1; i >= fromindex; i--)
                        {
                            if (i == readedLines.Length - 1 && readedLines[readedLines.Length - 1] == "")
                                continue;

                            lines[seat] = readedLines[i];
                            seat--;
                            if (seat < 0)
                                break;
                        }
                        if (startpos < readedPos)
                        {
                            readedPos = startpos;
                        }
                        else
                        {
                            //增大offset
                            offset *= 3;
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    finally
                    {
                        ArrayPool<byte>.Shared.Return(bs);
                    }
                }
            }


            return lines.Skip(seat + 1).ToArray();
        }

        public async Task StartOutput(Project project, IInfoOutput infoOutput, int preLines)
        {
            if (string.IsNullOrEmpty(project.LogPath))
            {
                await infoOutput.Output($"没有设置日志文件路径");
                return;
            }

            var logpath = Path.Combine(Global.AppConfig.Current.PublishRootPath, project.Name, project.LogPath);

            if (File.Exists(logpath) == false)
            {
                await infoOutput.Output($"找不到日志文件{logpath}");
                return;
            }

            var lines = await ReadLastLinesAsync(logpath, preLines);
            foreach( var item in lines)
            {
                await infoOutput.Output(item);
            }

            _fs = new FileStream(logpath , FileMode.Open , FileAccess.Read , FileShare.ReadWrite);
            //移到文件末尾
            _fs.Seek(0, SeekOrigin.End);
            try
            {
                using var sr = new StreamReader(_fs, Encoding.UTF8);
                string line;
                while (true)
                {
                    line = await sr.ReadLineAsync();
                    if (line == null)
                    {
                        break;
                    }

                    await infoOutput.Output(line);
                }
            }
            catch (Exception)
            {
                 
            }
            finally
            {
                _fs.Dispose();
                _fs = null;
            }
            
        }

       
    }
}
