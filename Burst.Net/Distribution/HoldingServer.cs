using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Burst.Net;

namespace Burst.Distribution
{
    /// <summary>
    /// 分布式管理类
    /// </summary>
    /// <remarks>
    /// 2010/09/19 Burst-最后一次修改
    /// </remarks>
    public class HoldingServer
    {
        protected static HoldingServer singleton;
        /// <summary>
        /// HoldingServer单一实例
        /// </summary>
        public static HoldingServer Singleton
        {
            get { return singleton; }
        }

        protected Listener lis;
        protected ResHost self;
        /// <summary>
        /// 代表本机的ResHost类型的对象
        /// </summary>
        public static ResHost Self
        {
            get { return singleton.self; }
        }
        protected int state;
        /// <summary>
        /// 当前分布式状态
        /// </summary>
        public static int State
        {
            get { return singleton.state; }
        }

        protected ServerObject server;
        /// <summary>
        /// 仅在本机是管理主机时为非null，表示管理主机特有的属性
        /// </summary>
        public static ServerObject Server
        {
            get { return singleton.server; }
        }
        protected ClientObject client;
        /// <summary>
        /// 仅在本机是管理主机时为null，表示非管理主机特有的属性
        /// </summary>
        public static ClientObject Client
        {
            get { return singleton.client; }
        }

        protected IPAddress remoteAddress;
        /// <summary>
        /// 管理主机的IP地址
        /// </summary>
        public static IPAddress RemoteAddress
        {
            get { return singleton.remoteAddress; }
        }
        protected IPAddress selfAddress;
        /// <summary>
        /// 本机的IP地址
        /// </summary>
        public static IPAddress SelfAddress
        {
            get { return singleton.selfAddress; }
        }

        /// <summary>
        /// HoldingServer初始化
        /// </summary>
        public HoldingServer()
        {
            singleton = this;
            self = new ResHost(new IPEndPoint(Functions.SelfIP, Configuration.Port), Dns.GetHostName());
            self.rGroup(Configuration.Groups);
            self.searchService = Functions.SearchService;
            lis = new Listener(Configuration.Port);
            lis.onAccept += new AcceptHandler(lis_onAccept);
            lis.onReceive += MessageDealer.UDPReceive;
            server = new ServerObject();
            client = new ClientObject();
            remoteAddress = Functions.RemoteIP;
            selfAddress = Functions.SelfIP;
            state = 0;
        }

        /// <summary>
        /// Tcp监听接受回调函数
        /// </summary>
        /// <param name="sender">接受的主机</param>
        void lis_onAccept(Peer sender)
        {
            if (state == 2)
            {
                sender.send(new Msg(21, client.remote));
                sender.close();
                return;
            }
            sender.onMessage += MessageDealer.Message;
            sender.onFinalDeal += MessageDealer.FinalDeal;
            sender.onPeerQuit += MessageDealer.PeerQuit;
        }

        /// <summary>
        /// 将本机添加到指定的分组
        /// </summary>
        /// <param name="name">分组名</param>
        public void AddGroup(string name)
        {
            self.addGroup(name);
            Configuration.Groups = self.groups.ToArray();
            if (state == 2)
                client.server.send(new Msg(11, new HostInfo()));
            else if (state == 1)
            {
                Group.send(new Msg(13));
                UpdateServices(HostGroup.getServices(self));
            }
        }
        /// <summary>
        /// 将本机从指定的分组中删除
        /// </summary>
        /// <param name="name">分组名</param>
        public void RemoveGroup(string name)
        {
            self.removeGroup(name);
            Configuration.Groups = self.groups.ToArray();
            if (state == 2)
                client.server.send(new Msg(11, new HostInfo()));
            else if (state == 1)
            {
                Group.send(new Msg(13));
                UpdateServices(HostGroup.getServices(self));
            }
        }
        /// <summary>
        /// 本机以管理主机身份开启分布式系统
        /// </summary>
        public void ServerStart()
        {
            client.remote = SelfAddress;
            lis.StartTcpAccept();
            state = 1;
            UpdateServices(HostGroup.getServices(self));
        }
        /// <summary>
        /// 本机以非管理主机身份开启分布式系统
        /// </summary>
        /// <param name="peer">管理主机</param>
        public void ClientStart(Peer peer)
        {
            client.remote = peer.info.ipep.Address;
            client.server = peer;
            peer.onFinalDeal += MessageDealer.FinalDeal;
            peer.onPeerQuit += MessageDealer.PeerQuit;
            peer.send(new Msg(11, new HostInfo()));
            lis.StartTcpAccept();
            state = 2;
        }
        /// <summary>
        /// 开始选举
        /// </summary>
        public void Elect()
        {
            Socket testSocket = HostManager.BeginConnect(true);
            if (testSocket != null)
                singleton.ClientStart(new Peer(testSocket, MessageDealer.Message));
            else
                singleton.ServerStart();
        }
        /// <summary>
        /// 为异步启动分布式系统提供支持
        /// </summary>
        protected void TrueStart()
        {
            if (Configuration.UseUDPToGetHosts)
                lis.StartUdpReceive();
            HostManager.Init();
            if (SelfAddress.Equals(RemoteAddress))
                TestServer();
            else
                TestClient();
        }
        /// <summary>
        /// 开启分布式系统
        /// </summary>
        /// <param name="ifAsync">是否异步</param>
        public static void Start(bool ifAsync)
        {
            if (singleton == null)
                new HoldingServer();
            if (ifAsync)
            {
                Thread t = new Thread(new ThreadStart(singleton.TrueStart));
                t.IsBackground = true;
                t.Start();
            }
            else
                singleton.TrueStart();
        }
        /// <summary>
        /// 尝试以管理主机身份开启分布式系统
        /// </summary>
        public void TestServer()
        {
            Socket testSocket = HostManager.TestConnectAll();
            if (testSocket != null)
                new Peer(testSocket, MessageDealer.Message).send(new Msg(22));
            else
                ServerStart();
        }
        /// <summary>
        /// 尝试以非管理主机身份开启分布式系统
        /// </summary>
        public void TestClient()
        {
            Socket serverSocket = NetUtils.TestConnect(new IPEndPoint(RemoteAddress, Configuration.Port));
            if (serverSocket != null)
                ClientStart(new Peer(serverSocket, MessageDealer.Message));
            else
                Elect();
        }

        /// <summary>
        /// 关闭分布式系统
        /// </summary>
        public static void End()
        {
            singleton.lis.StopUdpReceive();
            singleton.lis.StopTcpAccept();
            Peer.allClose();
            singleton.client.remote = null;
            singleton.state = 0;
        }

        protected string[] services;
        /// <summary>
        /// 获取与本机在同一个域中的WebService
        /// </summary>
        public static string[] Services
        {
            get
            {
                return singleton.services;
            }
        }
        /// <summary>
        /// 更新与本机在同一个域中的WebService
        /// </summary>
        /// <param name="services">新的WebService</param>
        public void UpdateServices(string[] services)
        {
            this.services = services;
        }
    }
}