using System;
using System.Configuration;
using System.Text;

namespace Burst
{
    public static class Settings
    {
        public static Encoding Encoding = System.Text.Encoding.GetEncoding("gb2312");
        public static int CacheMaxCount = 1000;
        public static int CacheMaxMilliSeconds = 1800000;
        public static byte[] CryptRgbIV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        public static string CryptRgbKey = "burst123";
    }
}
