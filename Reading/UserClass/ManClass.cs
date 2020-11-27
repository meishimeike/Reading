using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.IO;

namespace Reading
{
    class Config
    {
        public static string user = null;
        public static string userpath = null;
        public static string datapath = null;
        public static string configpath = null;
        public static string fiction = null;
        public static string Backcolor;
        public static string Fontcolor;
        public static string Font;
        public static string Sindex;
        public static string Logpath;
    }

    class fictioninfo
    {
        public string path;
        public string Title;
        public List<subtitinfo> subtitle = new List<subtitinfo>();
        public string Data;
    }

    struct subtitinfo
    {
        public string tit;
        public int index;
    }

    class Base64
    {
        public static string Base64Encode(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) 
            {
                return "";        
            }
            else
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
                return System.Convert.ToBase64String(plainTextBytes);
            }
        }

        public static string Base64Decode(string base64EncodedData)
        {
            if (string.IsNullOrEmpty(base64EncodedData))
            {
                return "";
            }
            else
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                return Encoding.UTF8.GetString(base64EncodedBytes);
            }
        }
    }

    /// <summary>
    /// 读写ini文件类
    /// </summary>
    class ReadWriteIni
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        
        /// <summary>
        /// 读INI文件
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <param name="Section">名称</param>
        /// <param name="Key">键</param>
        /// <returns></returns>
        public static string IniReadValue(string FilePath,string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, FilePath);
            return temp.ToString();
        }

        /// <summary>
        /// 写INI文件
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <param name="Section">名称</param>
        /// <param name="Key">键</param>
        /// <param name="Value">值</param>
        public static void IniWriteValue(string FilePath, string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, FilePath);
        }
    }

    public partial class MD5Helper
    {
        public static string EncryptString(string str)
        {
            MD5 md5 = MD5.Create();
            // 将字符串转换成字节数组
            byte[] byteOld = Encoding.UTF8.GetBytes(str);
            // 调用加密方法
            byte[] byteNew = md5.ComputeHash(byteOld);
            // 将加密结果转换为字符串
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteNew)
            {
                // 将字节转换成16进制表示的字符串，
                sb.Append(b.ToString("x2"));
            }
            // 返回加密的字符串
            return sb.ToString();
        }
    }

    public class EncodingType 
    {
        /// <summary>
        /// 取得一个文本文件的编码方式。如果无法在文件头部找到有效的前导符，Encoding.Default将被返回。
        /// </summary>
        /// <param name="fileName">文件名。</param>
        /// <returns></returns>
        public static Encoding GetEncoding(string fileName)
        {
            return GetEncoding(fileName, Encoding.Default);
        }
        /// <summary>
        /// 取得一个文本文件流的编码方式。
        /// </summary>
        /// <param name="stream">文本文件流。</param>
        /// <returns></returns>
        public static Encoding GetEncoding(FileStream stream)
        {
            return GetEncoding(stream, Encoding.Default);
        }
        /// <summary>
        /// 取得一个文本文件的编码方式。
        /// </summary>
        /// <param name="fileName">文件名。</param>
        /// <param name="defaultEncoding">默认编码方式。当该方法无法从文件的头部取得有效的前导符时，将返回该编码方式。</param>
        /// <returns></returns>
        public static Encoding GetEncoding(string fileName, Encoding defaultEncoding)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open);
            Encoding targetEncoding = GetEncoding(fs, defaultEncoding);
            fs.Close();
            return targetEncoding;
        }
        /// <summary>
        /// 取得一个文本文件流的编码方式。
        /// </summary>
        /// <param name="stream">文本文件流。</param>
        /// <param name="defaultEncoding">默认编码方式。当该方法无法从文件的头部取得有效的前导符时，将返回该编码方式。</param>
        /// <returns></returns>
        public static Encoding GetEncoding(FileStream stream, Encoding defaultEncoding)
        {
            Encoding targetEncoding = defaultEncoding;
            if (stream != null && stream.Length >= 2)
            {
                //保存文件流的前4个字节
                byte byte1 = 0;
                byte byte2 = 0;
                byte byte3 = 0;
                byte byte4 = 0;
                //保存当前Seek位置
                long origPos = stream.Seek(0, SeekOrigin.Begin);
                stream.Seek(0, SeekOrigin.Begin);

                int nByte = stream.ReadByte();
                byte1 = Convert.ToByte(nByte);
                byte2 = Convert.ToByte(stream.ReadByte());
                if (stream.Length >= 3)
                {
                    byte3 = Convert.ToByte(stream.ReadByte());
                }
                if (stream.Length >= 4)
                {
                    byte4 = Convert.ToByte(stream.ReadByte());
                }
                //根据文件流的前4个字节判断Encoding
                //Unicode {0xFF, 0xFE};
                //BE-Unicode {0xFE, 0xFF};
                //UTF8 = {0xEF, 0xBB, 0xBF};
                if (byte1 == 0xFE && byte2 == 0xFF)//UnicodeBe
                {
                    targetEncoding = Encoding.BigEndianUnicode;
                }
                if (byte1 == 0xFF && byte2 == 0xFE && byte3 != 0xFF)//Unicode
                {
                    targetEncoding = Encoding.Unicode;
                }
                if (byte1 == 0xEF && byte2 == 0xBB && byte3 == 0xBF)//UTF8
                {
                    targetEncoding = Encoding.UTF8;
                }
                //恢复Seek位置
                stream.Seek(origPos, SeekOrigin.Begin);
            }
            else
            {
                return GetEncoding((Stream)stream);
            }
            return targetEncoding;
        }



        // 新增加一个方法，解决了不带BOM的 UTF8 编码问题

        /// <summary>
        /// 通过给定的文件流，判断文件的编码类型
        /// </summary>
        /// <param name="fs">文件流</param>
        /// <returns>文件的编码类型</returns>
        public static Encoding GetEncoding(Stream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
            Encoding reVal = Encoding.Default;

            BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
            byte[] ss = r.ReadBytes(4);
            if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            else
            {
                if (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF)
                {
                    reVal = Encoding.UTF8;
                }
                else
                {
                    int i;
                    int.TryParse(fs.Length.ToString(), out i);
                    ss = r.ReadBytes(i);

                    if (IsUTF8Bytes(ss))
                        reVal = Encoding.UTF8;
                }
            }
            r.Close();
            return reVal;

        }

        /// <summary>
        /// 判断是否是不带 BOM 的 UTF8 格式
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1;　 //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式!");
            }
            return true;
        }
    }

    public class LogHelp 
    { 
        public static void Log (string msg)
        {
            File.AppendAllText(Config.Logpath, DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + " " + msg + "\r\n");
        }
    }  
}
