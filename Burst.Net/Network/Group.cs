using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Burst.Net
{
    /// <summary>
    /// 提供对主机的分组发送消息支持
    /// </summary>
    /// <remarks>
    /// 2010/09/19 Burst-最后一次修改
    /// </remarks>
    public class Group : NameObjectCollectionBase
    {
        protected static Group Singleton = new Group();
        protected static string broadcast = getNewGroup();
        /// <summary>
        /// 广播分组，所有主机会默认添加到该分组
        /// </summary>
        public static string BroadCast
        {
            get
            {
                return broadcast;
            }
        }

        /// <summary>
        /// 对单一分组发送消息
        /// </summary>
        /// <param name="msg">要发送的消息</param>
        /// <returns>是否成功</returns>
        public static bool send(Msg msg)
        {
            List<Peer> peers = Singleton.BaseGet(msg.target) as List<Peer>;
            if (!msg.valid || peers == null)
                return false;
            if (peers.Count == 0)
                return false;
            for (int i = 0; i < peers.Count; i++)
                peers[i].send(msg, false);
            return true;
        }

        /// <summary>
        /// 向分组中添加主机
        /// </summary>
        /// <param name="peer">要添加的主机</param>
        /// <param name="index">分组的标识</param>
        /// <returns>是否添加成功</returns>
        public static bool add(Peer peer, int index)
        {
            try
            {
                if (Singleton.BaseGet(index) == null)
                    Singleton.BaseSet(index, new List<Peer>());
                List<Peer> list = Singleton.BaseGet(index) as List<Peer>;
                list.Add(peer);
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// 向分组中添加主机
        /// </summary>
        /// <param name="peer">要添加的主机</param>
        /// <param name="name">分组的标识</param>
        /// <returns>是否添加成功</returns>
        public static bool add(Peer peer, string name)
        {
            try
            {
                if (Singleton.BaseGet(name) == null)
                    Singleton.BaseSet(name, new List<Peer>());
                List<Peer> list = Singleton.BaseGet(name) as List<Peer>;
                list.Add(peer);
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// 从分组中删除主机
        /// </summary>
        /// <param name="peer">要删除的主机</param>
        /// <param name="name">分组的标识</param>
        /// <returns>是否删除成功</returns>
        public static bool remove(Peer peer, string name)
        {
            try
            {
                if (Singleton.BaseGet(name) == null)
                    Singleton.BaseSet(name, new List<Peer>());
                List<Peer> list = Singleton.BaseGet(name) as List<Peer>;
                list.Remove(peer);
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// 从所有分组中删除主机
        /// </summary>
        /// <param name="peer">要删除的主机</param>
        public static void remove(Peer peer)
        {
            for (int i = 0; i < Singleton.Count; i++)
            {
                if (Singleton.BaseGet(i) as List<Peer> != null)
                {
                    List<Peer> list = Singleton.BaseGet(i) as List<Peer>;
                    list.Remove(peer);
                }
            }
        }
        /// <summary>
        /// 获取分组是否包含主机
        /// </summary>
        /// <param name="peer">要查看的主机</param>
        /// <param name="name">分组的标识</param>
        /// <returns>主机是否在分组中</returns>
        public static bool contains(Peer peer, string name)
        {
            try
            {
                if (Singleton.BaseGet(name) == null)
                    Singleton.BaseSet(name, new List<Peer>());
                List<Peer> list = Singleton.BaseGet(name) as List<Peer>;
                return list.Contains(peer);
            }
            catch { return false; }
        }

        /// <summary>
        /// 添加一个新的分组
        /// </summary>
        /// <returns>分组的标识</returns>
        public static string getNewGroup()
        {
            int p = 0;
            while (Singleton.BaseGet("g" + p) != null)
                p++;
            return "g" + p;
        }
        /// <summary>
        /// 获取指定分组的所有主机
        /// </summary>
        /// <param name="index">分组的标志</param>
        /// <returns></returns>
        public static Peer[] getPeers(string name)
        {
            if (Singleton.BaseGet(name) as List<Peer> == null)
                return null;
            else
                return (Singleton.BaseGet(name) as List<Peer>).ToArray();
        }
        /// <summary>
        /// 删除所有分组
        /// </summary>
        public static void clear()
        {
            Singleton.BaseClear();
        }
    }
}
