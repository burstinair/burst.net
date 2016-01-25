using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Burst.Net
{
    /// <summary>
    /// 提供同步方法所需的支持
    /// </summary>
    /// <remarks>
    /// 2010/09/19 Burst-最后一次修改
    /// </remarks>
    public class CallBack
    {
        /// <summary>
        /// 当前等待中的同步消息
        /// </summary>
        protected static Hashtable ht = new Hashtable();

        /// <summary>
        /// 通知同步返回
        /// </summary>
        /// <param name="sender">发送消息的主机</param>
        /// <param name="msg">消息</param>
        public static void doCallBack(Peer sender, Msg msg)
        {
            if (ht[msg.type] != null)
            {
                List<CallBack> list = ht[msg.type] as List<CallBack>;
                lock (list)
                {
                    for (int i = 0; i < list.Count; i++)
                        list[i].data = msg.data;
                    ht[msg.type] = null;
                }
            }
        }

        protected Object data = null;

        /// <summary>
        /// 调用同步方法
        /// </summary>
        /// <param name="type">消息类型</param>
        /// <returns>消息的返回值</returns>
        public static Object getSync(int type)
        {
            if (ht[type] == null)
                ht[type] = new List<CallBack>();
            CallBack waitCallBack = new CallBack();
            List<CallBack> list = ht[type] as List<CallBack>;
            lock (list)
            {
                list.Add(waitCallBack);
            }
            while (waitCallBack.data == null)
                Thread.Sleep(50);
            return waitCallBack.data;
        }
    }
}
