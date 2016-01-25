using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Web;
using System.Globalization;

using Burst.Distribution;

namespace Burst.Net
{
    /// <summary>
    /// Network相关工具类
    /// </summary>
    /// <remarks>
    /// 2010/09/19 Burst-最后一次修改
    /// </remarks>
    public static class NetUtils
    {
        /// <summary>
        /// 将序列化的字节数组转化为对象
        /// </summary>
        /// <param name="data">序列化的字节数组</param>
        /// <returns>转化好的对象</returns>
        public static Object ToObject(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryFormatter bf = new BinaryFormatter();
            try
            {
                return bf.Deserialize(ms);
            }
            catch { return null; }
        }
        /// <summary>
        /// 将一个对象序列化为字节数组
        /// </summary>
        /// <param name="data">要序列化的对象</param>
        /// <returns>序列化好的字节数组</returns>
        public static byte[] ToBytes(Object data)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, data);
            return ms.ToArray();
        }

        public static IPAddress GetMinIPv4Address()
        {
            IPAddress res = null;
            foreach (var ip in Dns.GetHostAddresses(""))
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    if (res == null)
                        res = ip;
                    else
                    {
                        var ipb = ip.GetAddressBytes();
                        var resb = res.GetAddressBytes();
                        for(int i = 0; i < 4; i++)
                            if (ipb[i] < resb[i])
                            {
                                res = ip;
                                break;
                            }
                    }
                }
            return res;
        }
        public static IPAddress GetFirstIPv4Address()
        {
            foreach (var ip in Dns.GetHostAddresses(""))
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip;
            return null;
        }

        /// <summary>
        /// 获取用于局域网广播的IPEndPoint
        /// </summary>
        /// <param name="port">广播端口</param>
        /// <returns>用于广播的IPEndPoint</returns>
        public static IPEndPoint BroadCastPoint(int port)
        {
            return new IPEndPoint(IPAddress.Broadcast, port);
        }

        /// <summary>
        /// 根据IP地址生成PeerID
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <returns>得到的PeerID</returns>
        public static PeerID GetIDByAddress(IPAddress ip)
        {
            return new PeerID(ip.GetHashCode());
        }

        /// <summary>
        /// 发送Udp数据
        /// </summary>
        /// <param name="data">要发送的数据</param>
        /// <param name="ipep">要发送到的IPEndPoint</param>
        /// <returns>是否成功</returns>
        public static bool SendUDP(Object data, IPEndPoint ipep)
        {
            try
            {
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                s.SendTo(ToBytes(data), ipep);
                s.Close();
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// 使用Udp进行局域网广播
        /// </summary>
        /// <param name="data">要广播的数据</param>
        /// <param name="port">要广播的端口</param>
        /// <returns>是否成功</returns>
        public static bool UDPBroadCast(Object data, int port)
        {
            try
            {
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
                s.SendTo(ToBytes(data), BroadCastPoint(port));
                s.Close();
                return true;
            }
            catch { return false; }
        }

        private static bool isConnected;
        private static ManualResetEvent to;
        private static void onConnect(IAsyncResult r)
        {
            if (isConnected)
                return;
            Socket s = r.AsyncState as Socket;
            try
            {
                s.EndConnect(r);
                isConnected = true;
            }
            catch (SocketException) { isConnected = false; }
            to.Set();
        }
        private static Socket TestConnect_once(IPEndPoint ipep)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            to = new ManualResetEvent(false);
            isConnected = false;
            try
            {
                IAsyncResult r = s.BeginConnect(ipep, new AsyncCallback(onConnect), s);
                to.WaitOne(Configuration.Timeout);
                if(isConnected)
                    return s;
            }
            catch { }
            return null;
        }
        /// <summary>
        /// 尝试使用Tcp连接到指定的IPEndPoint
        /// </summary>
        /// <param name="ipep">要尝试连接到的IPEndPoint</param>
        /// <returns>连接成功时返回连接的Socket，否则返回null</returns>
        public static Socket TestConnect(IPEndPoint ipep)
        {
            for (int i = 0; i < 3; i++)
            {
                Socket s = TestConnect_once(ipep);
                if (s != null)
                    return s;
            }
            return null;
        }
        /// <summary>
        /// 尝试连接到指定的一组IP地址指定的端口
        /// </summary>
        /// <param name="adds">要尝试连接到的IP地址</param>
        /// <param name="port">要尝试连接到的端口</param>
        /// <returns>连接成功时返回连接的Socket，否则返回null</returns>
        public static Socket TestConnect(IPAddress[] adds, int port)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            to = new ManualResetEvent(false);
            isConnected = false;
            try
            {
                IAsyncResult r = s.BeginConnect(adds, port, new AsyncCallback(onConnect), s);
                to.WaitOne(Configuration.Timeout);
                if (isConnected)
                    return s;
            }
            catch { }
            return null;
        }

        public static bool CanTcpBound(int Port)
        {
            try
            {
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(new IPEndPoint(IPAddress.Any, Port));
                socket.Dispose();
            }
            catch
            {
                return false;
            }
            return true;
        }
        public static bool CanUdpBound(int Port)
        {
            try
            {
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.Bind(new IPEndPoint(IPAddress.Any, Port));
                socket.Dispose();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
