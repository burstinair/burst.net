using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Burst.Distribution
{
    /// <summary>
    /// 表示主机列表
    /// </summary>
    /// <remarks>
    /// 2010/09/19 Burst-最后一次修改
    /// </remarks>
    [Serializable]
    public class HostList : NameObjectCollectionBase
    {
        /// <summary>
        /// 向列表中添加主机
        /// </summary>
        /// <param name="host">要添加的主机</param>
        public void Add(ResHost host)
        {
            BaseSet(host.id.ToString(), host);
        }
        /// <summary>
        /// 向列表中添加其他主机列表
        /// </summary>
        /// <param name="hostlist">要添加的主机列表</param>
        public void Add(HostList hostlist)
        {
            if (hostlist == null)
                return;
            for (int i = 0; i < hostlist.Count; i++)
                Add(hostlist[i]);
        }
        /// <summary>
        /// 提供索引
        /// </summary>
        /// <param name="id">主机的ID</param>
        /// <returns>得到的主机</returns>
        public ResHost this[string id]
        {
            get
            {
                try
                {
                    return BaseGet(id) as ResHost;
                }
                catch { return null; }
            }
            set { BaseSet(id, value); }
        }
        /// <summary>
        /// 提供索引
        /// </summary>
        /// <param name="index">主机在列表中的位置</param>
        /// <returns>得到的主机</returns>
        public ResHost this[int index]
        {
            get
            {
                try
                {
                    if (sortlist == null)
                        return BaseGet(index) as ResHost;
                    else
                        return BaseGet(sortlist[index]) as ResHost;
                }
                catch { return null; }
            }
            set { BaseSet(index, value); }
        }
        /// <summary>
        /// 从列表中删除指定id的主机
        /// </summary>
        /// <param name="id"></param>
        public void Remove(string id)
        {
            BaseRemove(id);
        }
        protected List<int> sortlist;
        /// <summary>
        /// 对列表排序
        /// </summary>
        public void Sort()
        {
            sortlist = new List<int>();
            for (int i = 0; i < Count; i++)
                sortlist.Add(i);
            sortlist.Sort(new Comparison<int>(compare));
        }
        protected int compare(int a, int b)
        {
            return (BaseGet(b) as ResHost).id - (BaseGet(a) as ResHost).id;
        }

        /// <summary>
        /// 从Xml格式的字符串生成HostList对象
        /// </summary>
        /// <param name="data">Xml格式的字符串</param>
        /// <returns>生成的HostList对象</returns>
        public static HostList FromXml(string data)
        {
            HostList res = new HostList();
            XElement xe = XElement.Parse(data);
            foreach (XElement c in xe.Elements())
                res.Add(ResHost.FromXml(c));
            return res;
        }
        /// <summary>
        /// 将HostList对象转化为Xml格式的字符串
        /// </summary>
        /// <returns>转化成的Xml格式的字符串</returns>
        public string ToXml()
        {
            XElement xe = new XElement("hostlist");
            for (int i = 0; i < Count; i++)
                xe.Add(this[i].ToXml());
            return xe.ToString();
        }
    }
}
