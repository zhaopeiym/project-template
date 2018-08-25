using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpZipLib.Checksum;

namespace ProjectNameTemplate.Common.Helper
{
    public class ZipHelper
    {
        /// <summary>
        /// 压缩指定路径文件
        /// </summary>
        /// <param name="sourceFilePath">压缩指定父文件夹内的文件</param>
        /// <param name="destinationZipFilePath">压缩至指定路径，含文件名</param>
        public static void CreateZip(string sourceFilePath, string destinationZipFilePath, List<string> sourceFilePathList = null)
        {
            if (sourceFilePath[sourceFilePath.Length - 1] != Path.DirectorySeparatorChar)
                sourceFilePath += Path.DirectorySeparatorChar;

            ZipOutputStream zipStream = new ZipOutputStream(File.Create(destinationZipFilePath));
            zipStream.SetLevel(6);  // 压缩级别 0-9
            CreateZipFiles(sourceFilePath, zipStream, sourceFilePath, sourceFilePathList);

            zipStream.Finish();
            zipStream.Close();
        }

        /// <summary>
        /// 递归压缩文件
        /// </summary>
        /// <param name="sourceFilePath">待压缩的文件或文件夹路径</param>
        /// <param name="zipStream">打包结果的zip文件路径（类似 D:\WorkSpace\a.zip）,全路径包括文件名和.zip扩展名</param>
        /// <param name="staticFile"></param>
        /// <param name="sourceFilePathList">待压缩文件夹路径集合：有，则值压缩集合中的文件</param>
        private static void CreateZipFiles(string sourceFilePath, ZipOutputStream zipStream, string staticFile, List<string> sourceFilePathList = null)
        {
            Crc32 crc = new Crc32();
            string[] filesArray = Directory.GetFileSystemEntries(sourceFilePath);
            foreach (string file in filesArray)
            {
                if (Directory.Exists(file))                     //如果当前是文件夹，递归
                {
                    CreateZipFiles(file, zipStream, staticFile, sourceFilePathList);
                }
                else                                            //如果是文件，开始压缩
                {
                    if (sourceFilePathList != null)
                    {
                        //  如果当前文件目录不再压缩目录中，则不压缩
                        if (!sourceFilePathList.Where(x => file.Contains(x)).Any())
                            return;
                    }
                    FileStream fileStream = File.OpenRead(file);

                    byte[] buffer = new byte[fileStream.Length];
                    fileStream.Read(buffer, 0, buffer.Length);
                    string tempFile = file.Substring(staticFile.LastIndexOf("\\") + 1);
                    ZipEntry entry = new ZipEntry(tempFile);

                    entry.DateTime = DateTime.Now;
                    entry.Size = fileStream.Length;
                    fileStream.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    zipStream.PutNextEntry(entry);

                    zipStream.Write(buffer, 0, buffer.Length);
                }
            }
        }
    }
}
