using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Burst.Net
{
    /// <summary>
    /// 表示一台主机的ID
    /// </summary>
    /// <remarks>
    /// 2010/09/19 Burst-最后一次修改
    /// </remarks>
    [Serializable]
    public class PeerID
    {
        protected int id;

        [NonSerialized]
        protected string str;

        /// <summary>
        /// 返回PeerID的16进制表示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (str == null)
            {
                str = id.ToString("x");
                while (str.Length < 8)
                    str = "0" + str;
            }
            return str;
        }

        /// <summary>
        /// 使用int类型对PeerID进行初始化
        /// </summary>
        /// <param name="id">int类型的初始值</param>
        public PeerID(int id)
        {
            this.id = id;
            this.str = null;
        }

        /// <summary>
        /// 提供对PeerID大小比较的支持，表示小于号
        /// </summary>
        /// <param name="a">运算符号前的PeerID</param>
        /// <param name="b">运算符号后的PeerID</param>
        /// <returns>如果a < b，返回true，否则，返回false</returns>
        public static bool operator <(PeerID a, PeerID b)
        {
            return a.id < b.id;
        }
        /// <summary>
        /// 提供对PeerID大小比较的支持，表示大于号
        /// </summary>
        /// <param name="a">运算符号前的PeerID</param>
        /// <param name="b">运算符号后的PeerID</param>
        /// <returns>如果a > b，返回true，否则，返回false</returns>
        public static bool operator >(PeerID a, PeerID b)
        {
            return a.id > b.id;
        }
        /// <summary>
        /// 提供对PeerID大小比较的支持，表示小于等于号
        /// </summary>
        /// <param name="a">运算符号前的PeerID</param>
        /// <param name="b">运算符号后的PeerID</param>
        /// <returns>如果a <= b，返回true，否则，返回false</returns>
        public static bool operator <=(PeerID a, PeerID b)
        {
            return a.id <= b.id;
        }
        /// <summary>
        /// 提供对PeerID大小比较的支持，表示大于等于号
        /// </summary>
        /// <param name="a">运算符号前的PeerID</param>
        /// <param name="b">运算符号后的PeerID</param>
        /// <returns>如果a >= b，返回true，否则，返回false</returns>
        public static bool operator >=(PeerID a, PeerID b)
        {
            return a.id >= b.id;
        }
        /// <summary>
        /// 判断该PeerID是否等于指定对象
        /// </summary>
        /// <param name="obj">要与之比较的对象</param>
        /// <returns>该PeerID是否等于指定对象</returns>
        public override bool Equals(object obj)
        {
            if (obj as PeerID != null)
                return (obj as PeerID).id == id;
            return false;
        }
        /// <summary>
        /// 获取该PeerID对象的HashCode
        /// </summary>
        /// <returns>得到的HashCode</returns>
        public override int GetHashCode()
        {
            return 0;
        }
        /// <summary>
        /// 获取两个PeerID的和
        /// </summary>
        /// <param name="a">第一个PeerID</param>
        /// <param name="b">第二个PeerID</param>
        /// <returns>两个PeerID的和</returns>
        public static string operator +(PeerID a, PeerID b)
        {
            return a.ToString() + b.ToString();
        }
        /// <summary>
        /// 获取两个PeerID的差
        /// </summary>
        /// <param name="a">第一个PeerID</param>
        /// <param name="b">第二个PeerID</param>
        /// <returns>两个PeerID的差</returns>
        public static int operator -(PeerID a, PeerID b)
        {
            if (a.id > b.id)
                return 1;
            else if (a.id < b.id)
                return -1;
            return 0;
        }
    }
}
