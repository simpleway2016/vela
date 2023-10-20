namespace VelaService
{
    using System;
    using System.Buffers;
    using System.IO;
    using System.IO.Compression;
    using System.Net.WebSockets;
    using System.Security.Cryptography;
    using System.Text;
    public class IOUtility
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
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string subfolder in Directory.GetDirectories(folderPath))
            {
                DeleteFolder(subfolder);
            }

            Directory.Delete(folderPath, false);
        }
        public static string GetHashString(byte[] data)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(data);
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

                File.Copy(file, Path.Combine(target, name), true);
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
                        IOUtility.DeleteFile(tofile);
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
