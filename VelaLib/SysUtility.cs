namespace VelaLib
{
    using LibGit2Sharp;
    using System;
    using System.Buffers;
    using System.IO;
    using System.IO.Compression;
    using System.IO.Pipelines;
    using System.Net.Sockets;
    using System.Net.WebSockets;
    using System.Security.Cryptography;
    using System.Text;
    public static class WebSocketExtens
    {
        public static Task<string> ReadString(this WebSocket webSocket)
        {
            return ReadString(webSocket, CancellationToken.None);
        }
        public static async Task<string> ReadString(this WebSocket webSocket,CancellationToken cancellationToken)
        {
            byte[] data = ArrayPool<byte>.Shared.Rent(4096);
            List<byte> list = null;
            try
            {
                var buffer = new ArraySegment<byte>(data);
                int len;
                while (true)
                {
                    var ret = await webSocket.ReceiveAsync(buffer, cancellationToken);
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
                    len = ret.Count;
                    if (ret.EndOfMessage)
                    {
                        if (list != null)
                        {
                            list.AddRange(buffer.Slice(0, ret.Count));
                        }
                        break;
                    }
                    else if (len > 0)
                    {
                        if (list == null)
                            list = new List<byte>();
                        list.AddRange(buffer.Slice(0, ret.Count));
                        //if (list.Count > 102400) //不需要判断这个，因为有些filelist很大
                        //{
                        //    list.Clear();
                        //    throw new Exception("websocket data is too big");
                        //}
                    }
                }
                if (list != null)
                    return Encoding.UTF8.GetString(list.ToArray());
                else
                    return Encoding.UTF8.GetString(data, 0, len);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(data);
                list?.Clear();
            }
        }

        public static async Task<byte[]> ReadBytes(this WebSocket webSocket)
        {
            byte[] data = ArrayPool<byte>.Shared.Rent(4096);
            List<byte> list = null;
            try
            {
                var buffer = new ArraySegment<byte>(data);
                int len;
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
                    len = ret.Count;
                    if (ret.EndOfMessage)
                    {
                        if (list != null)
                        {
                            list.AddRange(buffer.Slice(0, ret.Count));
                        }
                        break;
                    }
                    else if (len > 0)
                    {
                        if (list == null)
                            list = new List<byte>();
                        list.AddRange(buffer.Slice(0, ret.Count));
                        if (list.Count > 102400)
                        {
                            list.Clear();
                            throw new Exception("websocket data is too big");
                        }
                    }
                }
                if (list != null)
                    return list.ToArray();
                else
                    return buffer.Slice(0, len).ToArray();
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(data);
                list?.Clear();
            }
        }

        public static Task SendString(this WebSocket webSocket, string text)
        {
            var senddata = Encoding.UTF8.GetBytes(text);
            return webSocket.SendAsync(new ArraySegment<byte>(senddata), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        public static Task SendString(this WebSocket webSocket, string text, CancellationToken cancellationToken)
        {
            var senddata = Encoding.UTF8.GetBytes(text);
            return webSocket.SendAsync(new ArraySegment<byte>(senddata), WebSocketMessageType.Text, true, cancellationToken);
        }
        public static Task SendBytes(this WebSocket webSocket, byte[] data , int? length = null)
        {
            if (length != null)
            {
                return webSocket.SendAsync(new ArraySegment<byte>(data , 0 , length.Value), WebSocketMessageType.Binary, true, CancellationToken.None);
            }
            else
            {
                return webSocket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Binary, true, CancellationToken.None);
            }
        }

        public static Task SendBytes(this WebSocket webSocket, byte[] data, CancellationToken cancellationToken, int? length = null)
        {
            if (length != null)
            {
                return webSocket.SendAsync(new ArraySegment<byte>(data, 0, length.Value), WebSocketMessageType.Binary, true, cancellationToken);
            }
            else
            {
                return webSocket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Binary, true, cancellationToken);
            }
        }
    }

    public static class IOExtension
    {
        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string GetMessage(this Exception exception)
        {
            List<string> ret = new List<string>();
            ret.Add(exception.Message);

            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
                ret.Add(exception.Message);
            }
            return string.Join("\r\n" , ret);
        }
        public static string GetString(this ReadOnlySequence<byte> buffer)
        {
            if (buffer.IsSingleSegment)
            {
                return Encoding.UTF8.GetString(buffer.First.Span);
            }
            else
            {
                var len = (int)buffer.Length;
                var data = ArrayPool<byte>.Shared.Rent(len);
                Memory<byte> memory = new Memory<byte>(data);
                try
                {
                    foreach (var block in buffer)
                    {
                        block.CopyTo(memory);
                        memory = memory.Slice(block.Length);
                    }
                    return Encoding.UTF8.GetString(data, 0, len);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(data);
                }
            }
        }

        public static async Task<string> ReadLineAsync(this System.IO.Pipelines.PipeReader pipeReader)
        {
            ReadResult ret;
            SequencePosition? position;
            string line;
            byte n = (byte)'\n';
            ReadOnlySequence<byte> block;
            while (true)
            {
                ret = await pipeReader.ReadAsync();
                var buffer = ret.Buffer;

                position = buffer.PositionOf(n);
                if (position != null)
                {
                    block = buffer.Slice(0, position.Value);

                    line = block.GetString().Trim();

                    // 告诉PipeReader已经处理多少缓冲
                    pipeReader.AdvanceTo(buffer.GetPosition(1, position.Value));
                    return line;
                }
                else
                {
                    if (ret.IsCompleted)
                    {
                        if (buffer.Length > 0)
                        {
                            pipeReader.AdvanceTo(buffer.End);
                            return buffer.GetString().Trim();
                        }
                        return null;
                    }
                    // 告诉PipeReader已经处理多少缓冲
                    pipeReader.AdvanceTo(buffer.Start, buffer.End);
                }

            }
        }
    }
    public class SysUtility
    {
        public static void DeleteFile(string file)
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }
        public static void DeleteOldFiles(string folder,int days)
        {
            var files = Directory.GetFiles(folder);
            foreach (var file in files)
            {
                var fileinfo = new FileInfo(file);
                if ((DateTime.Now - fileinfo.CreationTime).TotalDays > days)
                {
                    try
                    {
                        File.SetAttributes(file, FileAttributes.Normal);
                        File.Delete(file);
                    }
                    catch
                    {
                    }
                }
            }
        }
        public static byte[] Compress(byte[] data)
        {
            using (MemoryStream compressedStream = new MemoryStream())
            {
                using (DeflateStream deflateStream = new DeflateStream(compressedStream, CompressionMode.Compress))
                {
                    deflateStream.Write(data, 0, data.Length);
                }
                return compressedStream.ToArray();
            }
        }

        public static byte[] Decompress(byte[] compressedData)
        {
            using (MemoryStream decompressedStream = new MemoryStream())
            {
                using (MemoryStream compressedStream = new MemoryStream(compressedData))
                {
                    using (DeflateStream deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
                    {
                        deflateStream.CopyTo(decompressedStream);
                    }
                }
                return decompressedStream.ToArray();
            }
        }
        public static void Decompress(Stream compressedStream, Stream saveStream)
        {
            using (DeflateStream deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            {
                deflateStream.CopyTo(saveStream);
            }
        }

        public static async Task<string> CalculateMD5Async(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hash =await md5.ComputeHashAsync(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLower();
                }
            }
        }

        public static string GetString(ReadOnlySequence<byte> buffer)
        {
            if (buffer.IsSingleSegment)
            {
                return Encoding.UTF8.GetString(buffer.First.Span);
            }
            else
            {
                var len = (int)buffer.Length;
                var data = ArrayPool<byte>.Shared.Rent(len);
                Memory<byte> memory = new Memory<byte>(data);
                try
                {
                    foreach (var block in buffer)
                    {
                        block.CopyTo(memory);
                        memory = memory.Slice(block.Length);
                    }
                    return Encoding.UTF8.GetString(data, 0, len);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(data);
                }
            }
        }

        public static void DeleteFolder(string folderPath)
        {
            foreach (string file in Directory.GetFiles(folderPath))
            {
                DeleteFile(file);
            }

            foreach (string subfolder in Directory.GetDirectories(folderPath))
            {
                DeleteFolder(subfolder);
            }

            Directory.Delete(folderPath, false);
        }

        /// <summary>
        /// 删除子文件和文件夹
        /// </summary>
        /// <param name="folderPath"></param>
        public static void DeleteSubFiles(string folderPath)
        {
            foreach (string file in Directory.GetFiles(folderPath))
            {
                DeleteFile(file);
            }

            foreach (string subfolder in Directory.GetDirectories(folderPath))
            {
                DeleteFolder(subfolder);
            }
        }
        public static string GetHashString(byte[] data)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(data);
                return ConvertBytesToHexString(hashBytes);
            }
        }
        public static string GetMD5String(byte[] data)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(data);
                return ConvertBytesToHexString(hashBytes);
            }
        }

        private static string ConvertBytesToHexString(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2")); // "x2" means format as hexadecimal with 2 digits
            }
            return builder.ToString();
        }

        /// <summary>
        /// 获取目录下的文件列表
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static string[] GetFolderFiles(string folder, string baseFolder, string[] includeFolders = null)
        {
            List<string> ret = new List<string>();
            var dirs = Directory.GetDirectories(folder);
            foreach (var dir in dirs)
            {
                if (new DirectoryInfo(dir).Attributes.HasFlag(FileAttributes.Hidden) == false)
                {
                    var reldir = Path.GetRelativePath(baseFolder, dir);
                    if (includeFolders == null || includeFolders.Contains(reldir))
                    {
                        var list = GetFolderFiles(dir, baseFolder, includeFolders);
                        if (list.Length > 0)
                        {
                            ret.AddRange(list);
                        }
                    }
                }
            }

            var files = Directory.GetFiles(folder);
            foreach (var file in files)
            {
                var path = Path.GetRelativePath(baseFolder, file).Replace("\\", "/");
                ret.Add(path);
            }

            return ret.ToArray();
        }

        public static void CopyFolder(string source, string target)
        {
            if (Directory.Exists(target) == false)
                Directory.CreateDirectory(target);

            var dirs = Directory.GetDirectories(source);
            foreach (var dir in dirs)
            {
                var name = Path.GetFileName(dir);
                CopyFolder(dir, Path.Combine(target, name));
            }

            var files = Directory.GetFiles(source);
            foreach (var file in files)
            {
                var name = Path.GetFileName(file);
                var targetFile = Path.Combine(target, name);
                try
                {
                    if (File.Exists(targetFile))
                    {
                        DeleteFile(targetFile);
                    }
                    File.Copy(file, targetFile, true);
                }
                catch (Exception ex)
                {
                    throw new Exception($"拷贝文件到{targetFile}失败,{ex.Message}");
                }
            }
        }

        public static void CopyFilesWithoutZip(string source, string targetFolder, string[] excludeExts)
        {
            if (Directory.Exists(targetFolder) == false)
                Directory.CreateDirectory(targetFolder);

           
            var files = Directory.GetFiles(source);
            foreach (var file in files)
            {
                if (excludeExts.Contains( Path.GetExtension(file) ))
                    continue;

                var name = Path.GetFileName(file);
                var tofile = Path.Combine(targetFolder, name);
                try
                {
                    if (File.Exists(tofile))
                    {
                        SysUtility.DeleteFile(tofile);
                    }
                    File.Copy(file, tofile, true);
                }
                catch 
                {
                }
            }
        }
    }

}
