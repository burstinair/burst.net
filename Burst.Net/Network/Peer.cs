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
    /// ��ʾ�����е�һ̨����������������
    /// </summary>
    /// <remarks>
    /// 2010/09/19 Burst-���һ���޸�
    /// </remarks>
    public class Peer
    {
        protected static List<Peer> peers = new List<Peer>();

        /// <summary>
        /// ��ȡָ��������
        /// </summary>
        /// <param name="match">�������Ҵ���</param>
        /// <returns>�ҵ�������</returns>
        public static Peer find(Predicate<Peer> match)
        {
            return peers.Find(match);
        }
        /// <summary>
        /// ��ȡָ��ID������
        /// </summary>
        /// <param name="id">����ID</param>
        /// <returns>�ҵ�������</returns>
        public static Peer find(PeerID id)
        {
            for (int i = 0; i < peers.Count;i++ )
                if(peers[i].info.id == id)
                    return peers[i];
            return null;
        }

        /// <summary>
        /// �ر���������������
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
        /// ���յ����Ը�������Tcp��Ϣʱ����
        /// </summary>
        public event MessageEventHandler onMessage;
        /// <summary>
        /// ��Ҫ���͵�����������Ϣ��Ҫ������ʱ����
        /// </summary>
        public event FinalDealHandler onFinalDeal;
        /// <summary>
        /// ��ʧȥ�Ը�����������ʱ����
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
        /// ����������Ϣ
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
        /// ��ȡ�������Ƿ���ָ��������
        /// </summary>
        /// <param name="name">ָ���ķ�����</param>
        /// <returns>�������Ƿ���ָ���ķ�����</returns>
        public bool isInGroup(string name)
        {
            return Group.contains(this, name);
        }

        /// <summary>
        /// ��ʼ��Peer
        /// </summary>
        /// <param name="socket">���ӵ�Socket</param>
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
        /// ��ʼ��Peer
        /// </summary>
        /// <param name="socket">���ӵ�Socket</param>
        /// <param name="onMessage">��onMessage�¼��ĳ�ʼ��</param>
        public Peer(Socket socket, MessageEventHandler onMessage) : this(socket)
        {
            this.onMessage += onMessage;
        }

        protected void MessageDealer(Object data)
        {
            onMessage(this, data as Msg);
        }
        /// <summary>
        /// ����Tcp��Ϣ�Ľ���
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
        /// ������������ָ������
        /// </summary>
        /// <param name="name">Ҫ����ķ�����</param>
        /// <returns>�Ƿ�ɹ�</returns>
        public bool addInto(string name)
        {
            return Group.add(this, name);
        }
        /// <summary>
        /// ����������ָ��������ɾ��
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>�Ƿ�ɹ�</returns>
        public bool quitFrom(string name)
        {
            return Group.remove(this, name);
        }

        /// <summary>
        /// �رնԸ�����������
        /// </summary>
        /// <returns>�Ƿ�ɹ�</returns>
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
        /// �Ը�����ʹ���첽��������һ����Ϣ
        /// </summary>
        /// <param name="msg">Ҫ���͵���Ϣ</param>
        public void send(Msg msg)
        {
            Thread t = new Thread(new ParameterizedThreadStart(SendProc));
            t.IsBackground = true;
            t.Start(msg);
        }
        /// <summary>
        /// �Ը���������һ����Ϣ
        /// </summary>
        /// <param name="msg">Ҫ���͵���Ϣ</param>
        /// <param name="ifAsync">�Ƿ��첽</param>
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
