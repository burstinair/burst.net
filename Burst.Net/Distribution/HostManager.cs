using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Burst.Net;

namespace Burst.Distribution
{
    /// <summary>
    /// 主机列表管理类，储存静态及动态列表，并提供对其连接的方法，可以通过索引方便的访问主机列表
    /// </summary>
    /// <remarks>
    /// 2010/09/19 Burst-最后一次修改
    /// </remarks>
    public class HostManager
    {
        protected static HostManager singleton;
        /// <summary>
        /// HostManager单一实例
        /// </summary>
        public static HostManager Singleton
        {
            get { return singleton; }
        }
        protected HostList staticList, dynamicList;
        protected ReceiveHandler recHandler;

        /// <summary>
        /// HostManager初始化
        /// </summary>
        public HostManager()
        {
            singleton = this;
        }

        /// <summary>
        /// HostManager初始化
        /// </summary>
        public static void Init()
        {
            if (singleton == null)
                new HostManager();
            singleton.staticList = Configuration.StaticHostList;
            singleton.dynamicList = new HostList();
            if (Configuration.UseUDPToGetHosts)
                BeginDynamic();
        }
        /// <summary>
        /// 开启使用Udp动态获取局域网中的主机
        /// </summary>
        protected static void BeginDynamic()
        {
            NetUtils.UDPBroadCast(new Msg(1, HoldingServer.Self), Configuration.Port);
            Thread.Sleep(Configuration.Timeout);
        }

        /// <summary>
        /// 向动态列表中添加主机
        /// </summary>
        /// <param name="rh">要添加的主机</param>
        public static void AddDynamic(ResHost rh)
        {
            singleton.dynamicList.Add(rh);
        }
        /// <summary>
        /// 从动态列表中删除主机
        /// </summary>
        /// <param name="id">要删除的主机的ID</param>
        public static void RemoveDynamic(string id)
        {
            singleton.dynamicList.Remove(id);
        }
        /// <summary>
        /// 向静态列表中添加主机
        /// </summary>
        /// <param name="rh">要添加的主机</param>
        public static void AddStatic(ResHost rh)
        {
            singleton.staticList.Add(rh);
            Configuration.StaticHostList = singleton.staticList;
        }
        /// <summary>
        /// 从静态列表中删除主机
        /// </summary>
        /// <param name="id">要删除的主机的ID</param>
        public static void RemoveStatic(string id)
        {
            singleton.staticList.Remove(id);
            Configuration.StaticHostList = singleton.staticList;
        }

        protected HostList sortedHosts;
        protected int step;
        /// <summary>
        /// 开始尝试向主机列表中的其他主机的连接
        /// </summary>
        /// <param name="ifBefore">是否只向ID比本机大的主机尝试连接</param>
        /// <returns>连接成功时返回连接的Socket，失败时返回null</returns>
        public static Socket BeginConnect(bool ifBefore)
        {
            singleton.sortedHosts = new HostList();
            singleton.sortedHosts.Add(singleton.dynamicList);
            singleton.sortedHosts.Add(singleton.staticList);
            singleton.sortedHosts.Sort();
            singleton.step = 0;
            while (singleton.step < singleton.sortedHosts.Count)
            {
                if (singleton.sortedHosts[singleton.step].id <= HoldingServer.Self.id && ifBefore)
                    break;
                Socket testSocket = NetUtils.TestConnect(singleton.sortedHosts[singleton.step].hostIPEP);
                if (testSocket != null)
                    return testSocket;
                singleton.step++;
            }
            return null;
        }
        /// <summary>
        /// 尝试下一次连接
        /// </summary>
        /// <param name="ifBefore">是否只向ID比本机大的主机尝试连接</param>
        /// <returns>连接成功时返回连接的Socket，失败时返回null</returns>
        public static Socket NextConnect(bool ifBefore)
        {
            while (singleton.step < singleton.sortedHosts.Count)
            {
                if (singleton.sortedHosts[singleton.step].id <= HoldingServer.Self.id && ifBefore)
                    break;
                Socket testSocket = NetUtils.TestConnect(singleton.sortedHosts[singleton.step].hostIPEP);
                if (testSocket != null)
                    return testSocket;
                singleton.step++;
            }
            return null;
        }
        /// <summary>
        /// 尝试向主机列表中的所有主机进行连接
        /// </summary>
        /// <returns>连接成功时返回连接的Socket，失败时返回null</returns>
        public static Socket TestConnectAll()
        {
            List<IPAddress> adds = new List<IPAddress>();
            for (int i = 0; i < singleton.staticList.Count; i++)
                adds.Add(singleton.staticList[i].hostIPEP.Address);
            for (int i = 0; i < singleton.dynamicList.Count; i++)
                adds.Add(singleton.dynamicList[i].hostIPEP.Address);
            return NetUtils.TestConnect(adds.ToArray(), Configuration.Port);
        }

        /// <summary>
        /// 获取所有指定分组中的所有WebService
        /// </summary>
        /// <param name="groups">指定的分组</param>
        /// <returns>得到的WebService</returns>
        public static string[] getServices(string[] groups)
        {
            return HostGroup.getServices(groups);
        }

        /// <summary>
        /// 主机列表中的主机数量
        /// </summary>
        public static int Count
        {
            get { return singleton.staticList.Count + singleton.dynamicList.Count; }
        }
        /// <summary>
        /// 提供索引
        /// </summary>
        /// <param name="index">主机在列表中的位置</param>
        /// <returns>得到的主机</returns>
        public ResHost this[int index]
        {
            get
            {
                if (index < staticList.Count)
                    return staticList[index];
                else if (index < staticList.Count + dynamicList.Count)
                    return dynamicList[index - staticList.Count];
                else
                    return null;
            }
        }
        /// <summary>
        /// 提供索引
        /// </summary>
        /// <param name="id">主机的ID</param>
        /// <returns>得到的主机</returns>
        public ResHost this[string id]
        {
            get
            {
                if (staticList[id] != null)
                    return staticList[id];
                else if (dynamicList[id] != null)
                    return dynamicList[id];
                else
                    return null;
            }
        }
    }
}
