using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;

namespace Burst.Net
{
    /// <summary>
    /// ��ʾ����ͨ�ż䴫�����Ϣ
    /// </summary>
    /// <remarks>
    /// 2010/09/19 Burst-���һ���޸�
    /// </remarks>
    [Serializable]
    public class Msg
    {
        /// <summary>
        /// ��ϢĿ����飬ֻ��ʹ��Group.sendʱ����
        /// </summary>
        [NonSerialized]
        public string target;

        /// <summary>
        /// ��Ϣ�Ƿ���ã����ΪFalse����Ϣ�������������ɴ���FinalDealʹ��
        /// </summary>
        [NonSerialized]
        public bool valid;

        /// <summary>
        /// ��Ϣ���������е�״̬����
        /// </summary>
        [NonSerialized]
        public Object signal;

        /// <summary>
        /// ��Ϣ����
        /// </summary>
        public Object data;
        /// <summary>
        /// ��Ϣ����
        /// </summary>
        public int type;

        /// <summary>
        /// ��ʼ��Msg
        /// </summary>
        /// <param name="target">Ŀ�����</param>
        /// <param name="type">��Ϣ����</param>
        /// <param name="data">��Ϣ����</param>
        public Msg(string target, int type, Object data)
        {
            this.target = target;
            this.type = type;
            this.data = data;
            this.valid = true;
        }
        /// <summary>
        /// ��ʼ��Msg
        /// </summary>
        /// <param name="type">��Ϣ����</param>
        public Msg(int type)
        {
            this.target = Group.BroadCast;
            this.type = type;
            this.data = null;
            this.valid = true;
        }
        /// <summary>
        /// ��ʼ��Msg
        /// </summary>
        /// <param name="type">��Ϣ����</param>
        /// <param name="data">��Ϣ����</param>
        public Msg(int type, Object data)
        {
            this.target = Group.BroadCast;
            this.type = type;
            this.data = data;
            this.valid = true;
        }

        /// <summary>
        /// ����Ϣ���л�������
        /// </summary>
        /// <param name="s">Ҫ���л�������</param>
        /// <returns>�Ƿ�ɹ�</returns>
        public bool toStream(Stream s)
        {
            try
            {
                new BinaryFormatter().Serialize(s, this);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static Msg fromStream(Stream s)
        {
            Msg msg;
            try
            {
                msg = new BinaryFormatter().Deserialize(s) as Msg;
            }
            catch
            {
                return null;
            }
            return msg;
        }
    }
}
