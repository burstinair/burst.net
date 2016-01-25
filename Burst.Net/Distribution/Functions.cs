using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Burst.Distribution
{
    /// <summary>
    /// 分布式工具类
    /// </summary>
    /// <remarks>
    /// 2010/09/19 Burst-最后一次修改
    /// </remarks>
    public static class Functions
    {
        /// <summary>
        /// 本机SearchService地址
        /// </summary>
        public static string SearchService;
        /// <summary>
        /// 本机IP
        /// </summary>
        public static IPAddress SelfIP;
        /// <summary>
        /// 管理主机IP
        /// </summary>
        public static IPAddress RemoteIP;

        /*
        private static IPAddress GetIP()
        {
            IPAddress[] ips = Dns.GetHostAddresses("");
            for (int i = 0; i < ips.Length; i++)
            {
                if (ips[i].IsIPv6LinkLocal || ips[i].IsIPv6Multicast || ips[i].IsIPv6SiteLocal)
                    continue;
                if(ips[i].ToString().IndexOf("192.168") == 0)
                    return ips[i];
            }
            return null;
        }
        */
    }
}
