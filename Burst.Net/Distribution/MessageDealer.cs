using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Burst.Net;

namespace Burst.Distribution
{
    /// <summary>
    /// 消息处理类
    /// </summary>
    /// <remarks>
    /// 2010/09/19 Burst-最后一次修改
    /// </remarks>
    public static class MessageDealer
    {
        public static readonly MessageEventHandler Message = new MessageEventHandler(onMessage);
        public static readonly FinalDealHandler FinalDeal = new FinalDealHandler(onFinalDeal);
        public static readonly PeerQuitHandler PeerQuit = new PeerQuitHandler(onPeerQuit);
        public static readonly ReceiveHandler UDPReceive = new ReceiveHandler(onUDPReceive);

        /// <summary>
        /// 处理Udp消息
        /// </summary>
        /// <param name="ipep">消息来源IPEndPoint</param>
        /// <param name="data">消息内容</param>
        public static void onUDPReceive(IPEndPoint ipep, Object data)
        {
            Msg msg = data as Msg;
            switch (msg.type)
            {
                case 1:
                    if (msg.data as ResHost == null)
                        return;
                    if (!(msg.data as ResHost).id.Equals(HoldingServer.Self.id))
                    {
                        HostManager.AddDynamic(msg.data as ResHost);
                        NetUtils.SendUDP(new Msg(2, HoldingServer.Self), new IPEndPoint(ipep.Address, Configuration.Port));
                    }
                    break;
                case 2:
                    if (msg.data as ResHost == null)
                        return;
                    if (!(msg.data as ResHost).id.Equals(HoldingServer.Self.id))
                        HostManager.AddDynamic(msg.data as ResHost);
                    break;
            }
        }

        /// <summary>
        /// 处理管理主机消息
        /// </summary>
        /// <param name="peer">消息来源主机</param>
        /// <param name="msg">消息</param>
        private static void ServerMessage(Peer peer, Msg msg)
        {
            switch (msg.type)
            {
                case 11:
                    HostInfo info = msg.data as HostInfo;
                    peer.info.id = info.id;
                    ResHost curHost = HostManager.Singleton[info.id.ToString()];
                    if (curHost == null)
                        return;
                    curHost.rGroup(info.groups);
                    curHost.searchService = info.SearchService;
                    HoldingServer.Singleton.UpdateServices(HostGroup.getServices(HoldingServer.Self));
                    Group.send(new Msg(13));
                    break;
                case 12:
                    peer.send(new Msg(13, HostGroup.getServices(msg.data as string[])));
                    break;
                case 22:
                    peer.send(new Msg(21, HoldingServer.Client.remote));
                    break;
            }
        }
        /// <summary>
        /// 处理其他消息
        /// </summary>
        /// <param name="peer">消息来源主机</param>
        /// <param name="msg">消息</param>
        private static void OtherMessage(Peer peer, Msg msg)
        {
            switch (msg.type)
            {
                case 21:
                    peer.close();
                    IPAddress add = msg.data as IPAddress;
                    Socket s = NetUtils.TestConnect(new IPEndPoint(add, Configuration.Port));
                    if (s == null)
                        HoldingServer.Singleton.TestServer();
                    else
                        HoldingServer.Singleton.ClientStart(new Peer(s, Message));
                    break;
            }
        }
        /// <summary>
        /// 处理非管理主机消息
        /// </summary>
        /// <param name="peer">消息来源主机</param>
        /// <param name="msg">消息</param>
        private static void ClientMessage(Peer peer, Msg msg)
        {
            switch (msg.type)
            {
                case 13:
                    HoldingServer.Singleton.UpdateServices(msg.data as string[]);
                    break;
                case 21:
                    peer.close();
                    IPAddress add = msg.data as IPAddress;
                    Socket s = NetUtils.TestConnect(new IPEndPoint(add, Configuration.Port));
                    if (s == null)
                        HoldingServer.Singleton.TestServer();
                    else
                        HoldingServer.Singleton.ClientStart(new Peer(s, Message));
                    break;
            }
        }
        /// <summary>
        /// 消息路由函数
        /// </summary>
        /// <param name="peer">消息来源主机</param>
        /// <param name="msg">消息</param>
        public static void onMessage(Peer peer, Msg msg)
        {
            if (HoldingServer.State == 1)
                ServerMessage(peer, msg);
            else if (HoldingServer.State == 2)
                ClientMessage(peer, msg);
            else
                OtherMessage(peer, msg);
            CallBack.doCallBack(peer, msg);
        }

        /// <summary>
        /// 处理部分消息的FinalDeal事件
        /// </summary>
        /// <param name="peer">消息要发送到的主机</param>
        /// <param name="msg">消息</param>
        public static void onFinalDeal(Peer peer, Msg msg)
        {
            switch (msg.type)
            {
                case 13:
                    if (peer.info.id == null)
                    {
                        msg.valid = false;
                        return;
                    }
                    msg.valid = true;
                    ResHost rh = HostManager.Singleton[peer.info.id.ToString()];
                    msg.data = HostGroup.getServices(rh);
                    break;
            }
        }

        /// <summary>
        /// 处理主机断开
        /// </summary>
        /// <param name="peer">断开的主机</param>
        public static void onPeerQuit(Peer peer)
        {
            if (HoldingServer.State == 1)
            {
                if (peer.info.id == null)
                    return;
                ResHost rh = HostManager.Singleton[peer.info.id.ToString()];
                if (rh != null)
                    rh.rGroup(new string[0]);
            }
            else if(peer == HoldingServer.Client.server)
                HoldingServer.Start(false);
        }
    }
}
