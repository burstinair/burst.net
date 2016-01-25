using System.Threading;
using System.Net;
using System.Net.Sockets;
using System;
using System.Text;
using System.Collections.Generic;

namespace Burst.Net
{
    public delegate void MessageEventHandler(Peer sender, Msg msg);
    public delegate void FinalDealHandler(Peer sender, Msg msg);
    public delegate void PeerQuitHandler(Peer sender);

    /// <summary>
    /// 表示网络中的一台主机及对它的连接
    /// </summary>
    /// <remarks>
    /// 2010/09/19 Burst-最后一次修改
    /// </remarks>
    public class Peer
    {
        protected static List<Peer> peers = new List<Peer>();

        /// <summary>
        /// 获取指定的主机
        /// </summary>
        /// <param name="match">主机查找代理</param>
        /// <returns>找到的主机</returns>
        public static Peer find(Predicate<Peer> match)
        {
            return peers.Find(match);
        }
        /// <summary>
        /// 获取指定ID的主机
        /// </summary>
        /// <param name="id">主机ID</param>
        /// <returns>找到的主机</returns>
        public static Peer find(PeerID id)
        {
            for (int i = 0; i < peers.Count;i++ )
                if(peers[i].info.id == id)
                    return peers[i];
            return null;
        }

        /// <summary>
        /// 关闭所有主机的连接
        /// </summary>
        public static void allClose()
        {
            for (int i = 0; i < peers.Count; i++)
            {
                peers[i].close();
            }
            peers.Clear();
        }

        /// <summary>
        /// 在收到来自该主机的Tcp消息时发生
        /// </summary>
        public event MessageEventHandler onMessage;
        /// <summary>
        /// 在要发送到该主机的消息将要被发送时发生
        /// </summary>
        public event FinalDealHandler onFinalDeal;
        /// <summary>
        /// 在失去对该主机的连接时发生
        /// </summary>
        public event PeerQuitHandler onPeerQuit;

        protected bool isActive;
        protected Socket socket;
        protected NetworkStream NS;

        /// <summary>
        /// 
        /// </summary>
        protected PeerInformation Information;
        /// <summary>
        /// 该主机的信息
        /// </summary>
        public PeerInformation info
        {
            get { return this.Information; }
            set
            {
                this.Information = value;
                if (this.Information.ipep == null)
                    this.Information.ipep = socket.RemoteEndPoint as IPEndPoint;
            }
        }

        /// <summary>
        /// 获取该主机是否在指定分组中
        /// </summary>
        /// <param name="name">指定的分组名</param>
        /// <returns>该主机是否在指定的分组中</returns>
        public bool isInGroup(string name)
        {
            return Group.contains(this, name);
        }

        /// <summary>
        /// 初始化Peer
        /// </summary>
        /// <param name="socket">连接的Socket</param>
        public Peer(Socket socket)
        {
            this.socket = socket;
            this.NS = new NetworkStream(socket);
            this.Information = new PeerInformation(socket.RemoteEndPoint);
            peers.Add(this);
            this.addInto(Group.BroadCast);
            this.isActive = true;
            Thread t = new Thread(new ThreadStart(ReceiveProcess));
            t.IsBackground = true;
            t.Start();
        }
        /// <summary>
        /// 初始化Peer
        /// </summary>
        /// <param name="socket">连接的Socket</param>
        /// <param name="onMessage">对onMessage事件的初始化</param>
        public Peer(Socket socket, MessageEventHandler onMessage) : this(socket)
        {
            this.onMessage += onMessage;
        }

        protected void MessageDealer(Object data)
        {
            onMessage(this, data as Msg);
        }
        /// <summary>
        /// 接收Tcp消息的进程
        /// </summary>
        protected void ReceiveProcess()
        {
            while (true)
            {
                Msg msg = Msg.fromStream(NS);
                if (msg == null)
                    break;
                if (onMessage != null)
                {
                    Thread t = new Thread(new ParameterizedThreadStart(MessageDealer));
                    t.IsBackground = true;
                    t.Start(msg);
                }
            }
            Dispose();
            if (isActive && onPeerQuit != null)
                onPeerQuit(this);
        }

        /// <summary>
        /// 将该主机假如指定分组
        /// </summary>
        /// <param name="name">要加入的分组名</param>
        /// <returns>是否成功</returns>
        public bool addInto(string name)
        {
            return Group.add(this, name);
        }
        /// <summary>
        /// 将该主机从指定分组中删除
        /// </summary>
        /// <param name="name">分组名</param>
        /// <returns>是否成功</returns>
        public bool quitFrom(string name)
        {
            return Group.remove(this, name);
        }

        /// <summary>
        /// 关闭对该主机的连接
        /// </summary>
        /// <returns>是否成功</returns>
        public bool close()
        {
            try
            {
                socket.Close();
                isActive = false;
                NS.Close();
                Dispose();
            }
            catch
            {
                return false;
            }
            return true;
        }

        protected void Dispose()
        {
            Group.remove(this);
        }

        protected void SendProc(Object data)
        {
            Msg msg = data as Msg;
            if (onFinalDeal != null)
                onFinalDeal(this, msg);
            if (msg.valid)
                msg.toStream(this.NS);
        }
        /// <summary>
        /// 对该主机使用异步方法发送一条消息
        /// </summary>
        /// <param name="msg">要发送的消息</param>
        public void send(Msg msg)
        {
            Thread t = new Thread(new ParameterizedThreadStart(SendProc));
            t.IsBackground = true;
            t.Start(msg);
        }
        /// <summary>
        /// 对该主机发送一条消息
        /// </summary>
        /// <param name="msg">要发送的消息</param>
        /// <param name="ifAsync">是否异步</param>
        public void send(Msg msg, bool ifAsync)
        {
            if (ifAsync)
            {
                Thread t = new Thread(new ParameterizedThreadStart(SendProc));
                t.IsBackground = true;
                t.Start(msg);
            }
            else
                SendProc(msg);
        }
    }
}
