using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Net;

namespace Burst.Distribution
{
    /// <summary>
    /// 分布式配置信息
    /// </summary>
    /// <remarks>
    /// 2010/09/19 Burst-最后一次修改
    /// </remarks>
    public class Configuration
    {
        protected static bool ifChanged;
        protected static Timer timer;

        /// <summary>
        /// 配置初始化
        /// </summary>
        public static void Initialize()
        {
            //port = Convert.ToInt32(fxApp.fxDISTRIBUTIONPORT);
            useUDPToGetHosts = true;
            groups = new string[] { "g1" };
            timeout = 2000;

            try
            {
                //initial
            }
            catch
            {
                staticHostList = new HostList();
            }

            timer = new Timer(30000);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            ifChanged = false;
        }

        protected static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (ifChanged)
            {
                ifChanged = false;
                SaveConfig();
            }
        }

        public static void LoadConfig()
        {
            
        }
        public static void SaveConfig()
        {
            
        }

        protected static HostList staticHostList;
        protected static bool useUDPToGetHosts;
        protected static int port;
        protected static int timeout;
        protected static string[] groups;

        /// <summary>
        /// 静态主机列表
        /// </summary>
        public static HostList StaticHostList
        {
            get{ return staticHostList; }
            set
            {
                staticHostList = value;
                ifChanged = true;
            }
        }
        /// <summary>
        /// 是否使用Udp动态获取局域网中的主机
        /// </summary>
        public static bool UseUDPToGetHosts
        {
            get { return useUDPToGetHosts; }
            set
            {
                useUDPToGetHosts = value;
                ifChanged = true;
            }
        }
        /// <summary>
        /// 分布式通信端口
        /// </summary>
        public static int Port
        {
            get { return port; }
            set
            {
                port = value;
                ifChanged = true;
            }
        }
        /// <summary>
        /// 分布式连接尝试超时时间
        /// </summary>
        public static int Timeout
        {
            get { return timeout; }
            set
            {
                timeout = value;
                ifChanged = true;
            }
        }
        /// <summary>
        /// 该主机属于的分组
        /// </summary>
        public static string[] Groups
        {
            get { return groups; }
            set
            {
                groups = value;
                ifChanged = true;
            }
        }
    }
}
