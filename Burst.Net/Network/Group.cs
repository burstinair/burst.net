using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Burst.Net
{
    /// <summary>
    /// �ṩ�������ķ��鷢����Ϣ֧��
    /// </summary>
    /// <remarks>
    /// 2010/09/19 Burst-���һ���޸�
    /// </remarks>
    public class Group : NameObjectCollectionBase
    {
        protected static Group Singleton = new Group();
        protected static string broadcast = getNewGroup();
        /// <summary>
        /// �㲥���飬����������Ĭ����ӵ��÷���
        /// </summary>
        public static string BroadCast
        {
            get
            {
                return broadcast;
            }
        }

        /// <summary>
        /// �Ե�һ���鷢����Ϣ
        /// </summary>
        /// <param name="msg">Ҫ���͵���Ϣ</param>
        /// <returns>�Ƿ�ɹ�</returns>
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
        /// ��������������
        /// </summary>
        /// <param name="peer">Ҫ��ӵ�����</param>
        /// <param name="index">����ı�ʶ</param>
        /// <returns>�Ƿ���ӳɹ�</returns>
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
        /// ��������������
        /// </summary>
        /// <param name="peer">Ҫ��ӵ�����</param>
        /// <param name="name">����ı�ʶ</param>
        /// <returns>�Ƿ���ӳɹ�</returns>
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
        /// �ӷ�����ɾ������
        /// </summary>
        /// <param name="peer">Ҫɾ��������</param>
        /// <param name="name">����ı�ʶ</param>
        /// <returns>�Ƿ�ɾ���ɹ�</returns>
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
        /// �����з�����ɾ������
        /// </summary>
        /// <param name="peer">Ҫɾ��������</param>
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
        /// ��ȡ�����Ƿ��������
        /// </summary>
        /// <param name="peer">Ҫ�鿴������</param>
        /// <param name="name">����ı�ʶ</param>
        /// <returns>�����Ƿ��ڷ�����</returns>
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
        /// ���һ���µķ���
        /// </summary>
        /// <returns>����ı�ʶ</returns>
        public static string getNewGroup()
        {
            int p = 0;
            while (Singleton.BaseGet("g" + p) != null)
                p++;
            return "g" + p;
        }
        /// <summary>
        /// ��ȡָ���������������
        /// </summary>
        /// <param name="index">����ı�־</param>
        /// <returns></returns>
        public static Peer[] getPeers(string name)
        {
            if (Singleton.BaseGet(name) as List<Peer> == null)
                return null;
            else
                return (Singleton.BaseGet(name) as List<Peer>).ToArray();
        }
        /// <summary>
        /// ɾ�����з���
        /// </summary>
        public static void clear()
        {
            Singleton.BaseClear();
        }
    }
}
