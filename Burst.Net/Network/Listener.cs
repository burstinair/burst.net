using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Burst.Net
{
    /// <summary>
    /// 异步接受连接请求代理
    /// </summary>
    /// <param name="sender">要接受的主机</param>
    public delegate void AcceptHandler(Peer sender);
    /// <summary>
    /// 异步Udp接收消息代理
    /// </summary>
    /// <param name="ipep">消息来源点</param>
    /// <param name="data">接收到的消息</param>
    public delegate void ReceiveHandler(IPEndPoint ipep, byte[] data);
    
    /// <summary>
    /// 提供对Socket监听的支持
    /// </summary>
    /// <remarks>
    /// 2010/09/19 Burst-最后一次修改
    /// </remarks>
    public class Listener
    {
        /// <summary>
        /// 在该Listener接受某主机时发生
        /// </summary>
        public event AcceptHandler onAccept;
        /// <summary>
        /// 在该Listener接收到Udp消息时发生
        /// </summary>
        public event ReceiveHandler onReceive;

        protected int state;
        protected IPEndPoint ipep;
        protected UdpClient udpListener;
        protected Socket tcpListener;

        public UdpClient UdpListener { get { return udpListener; } }
        public Socket TcpListener { get { return tcpListener; } }

        /// <summary>
        /// 初始化Listener
        /// </summary>
        /// <param name="port">要监听的端口</param>
        public Listener(int port)
        {
            this.ipep = new IPEndPoint(IPAddress.Any, port);
            state = 0;
        }
        /// <summary>
        /// 初始化Listener
        /// </summary>
        /// <param name="ipep">要监听的IPEndPoint</param>
        public Listener(IPEndPoint ipep)
        {
            this.ipep = ipep;
            state = 0;
        }

        /// <summary>
        /// 开始Tcp监听
        /// </summary>
        public void StartTcpAccept()
        {
            if (state % 10 == 0)
            {
                state += 1;
                tcpListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                tcpListener.Bind(ipep);
                tcpListener.Listen(int.MaxValue);
                tcpListener.BeginAccept(new AsyncCallback(Accept), null);
            }
        }
        /// <summary>
        /// 停止Tcp监听
        /// </summary>
        public void StopTcpAccept()
        {
            if (state % 10 == 1)
            {
                tcpListener.Close();
                state -= 1;
            }
        }
        /// <summary>
        /// 开始Udp监听
        /// </summary>
        public void StartUdpReceive()
        {
            if (state / 10 == 0)
            {
                state += 10;
                udpListener = new UdpClient(ipep.Port);
                udpListener.BeginReceive(new AsyncCallback(Receive), null);
            }
        }
        /// <summary>
        /// 停止Udp监听
        /// </summary>
        public void StopUdpReceive()
        {
            if (state / 10 == 1)
            {
                state -= 10;
                udpListener.Close();
            }
        }

        /// <summary>
        /// Tcp接受主机异步回调函数
        /// </summary>
        /// <param name="r">异步结果</param>
        protected void Accept(IAsyncResult r)
        {
            try
            {
                Socket remotePoint = tcpListener.EndAccept(r);
                Peer peer = new Peer(remotePoint);
                if (onAccept != null)
                    onAccept(peer);
                if (state % 10 == 1)
                    tcpListener.BeginAccept(new AsyncCallback(Accept), null);
            }
            catch { }
        }
        /// <summary>
        /// Udp接收消息异步回调函数
        /// </summary>
        /// <param name="r">异步结果</param>
        protected void Receive(IAsyncResult r)
        {
            try
            {
                IPEndPoint ipep = new IPEndPoint(0, 0);
                byte[] data = udpListener.EndReceive(r, ref ipep);
                if (onReceive != null)
                    onReceive(ipep, data);
                if (state / 10 == 1)
                    udpListener.BeginReceive(new AsyncCallback(Receive), null);
            }
            catch { }
        }
    }
}
