using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;

namespace Burst.Net
{
    /// <summary>
    /// 表示网络通信间传输的消息
    /// </summary>
    /// <remarks>
    /// 2010/09/19 Burst-最后一次修改
    /// </remarks>
    [Serializable]
    public class Msg
    {
        /// <summary>
        /// 消息目标分组，只在使用Group.send时有用
        /// </summary>
        [NonSerialized]
        public string target;

        /// <summary>
        /// 消息是否可用，如果为False，消息将不被发出，可搭配FinalDeal使用
        /// </summary>
        [NonSerialized]
        public bool valid;

        /// <summary>
        /// 消息发出过程中的状态对象
        /// </summary>
        [NonSerialized]
        public Object signal;

        /// <summary>
        /// 消息内容
        /// </summary>
        public Object data;
        /// <summary>
        /// 消息类型
        /// </summary>
        public int type;

        /// <summary>
        /// 初始化Msg
        /// </summary>
        /// <param name="target">目标分组</param>
        /// <param name="type">消息类型</param>
        /// <param name="data">消息内容</param>
        public Msg(string target, int type, Object data)
        {
            this.target = target;
            this.type = type;
            this.data = data;
            this.valid = true;
        }
        /// <summary>
        /// 初始化Msg
        /// </summary>
        /// <param name="type">消息类型</param>
        public Msg(int type)
        {
            this.target = Group.BroadCast;
            this.type = type;
            this.data = null;
            this.valid = true;
        }
        /// <summary>
        /// 初始化Msg
        /// </summary>
        /// <param name="type">消息类型</param>
        /// <param name="data">消息内容</param>
        public Msg(int type, Object data)
        {
            this.target = Group.BroadCast;
            this.type = type;
            this.data = data;
            this.valid = true;
        }

        /// <summary>
        /// 将消息序列化到流中
        /// </summary>
        /// <param name="s">要序列化到的流</param>
        /// <returns>是否成功</returns>
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
