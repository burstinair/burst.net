using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml.Linq;
using Burst.Net;

namespace Burst.Distribution
{
    /// <summary>
    /// 表示一台主机
    /// </summary>
    /// <remarks>
    /// 2010/09/19 Burst-最后一次修改
    /// </remarks>
    [Serializable]
    public class ResHost
    {
        /// <summary>
        /// 主机的IPEndPoint
        /// </summary>
        public IPEndPoint hostIPEP;
        /// <summary>
        /// 主机名
        /// </summary>
        public string hostName;
        /// <summary>
        /// 主机的ID
        /// </summary>
        public PeerID id;

        /// <summary>
        /// 主机的SearchService地址
        /// </summary>
        [NonSerialized]
        public string searchService;

        /// <summary>
        /// 主机所在的分组
        /// </summary>
        [NonSerialized]
        public List<string> groups;

        /// <summary>
        /// ResHost初始化
        /// </summary>
        /// <param name="ipep">主机的IPEndPoint</param>
        /// <param name="hostName">主机名</param>
        public ResHost(IPEndPoint ipep, string hostName)
        {
            this.hostIPEP = ipep;
            this.id = NetUtils.GetIDByAddress(ipep.Address);
            this.hostName = hostName;
        }

        /// <summary>
        /// 将主机添加到指定分组
        /// </summary>
        /// <param name="group">要添加到的分组的名字</param>
        public void addGroup(string group)
        {
            Peer peer = Peer.find(id);
            if (this.groups == null)
                this.groups = new List<string>();
            if (peer != null)
                peer.addInto(group);
            HostGroup.add(this, group);
            groups.Add(group);
        }
        /// <summary>
        /// 将主机从指定分组中删除
        /// </summary>
        /// <param name="group">要从中删除的分组的名字</param>
        public void removeGroup(string group)
        {
            Peer peer = Peer.find(id);
            if (this.groups == null)
                this.groups = new List<string>();
            if (peer != null)
                peer.quitFrom(group);
            HostGroup.remove(this, group);
            groups.Remove(group);
        }
        /// <summary>
        /// 刷新分组
        /// </summary>
        /// <param name="groups">新的分组</param>
        public void rGroup(string[] groups)
        {
            Peer peer = Peer.find(id);
            if(groups == null)
                groups = new string[0];
            if(this.groups == null)
                this.groups = new List<string>();
            for (int i = 0; i < this.groups.Count; i++)
                if (!groups.Contains(this.groups[i]))
                {
                    if (peer != null)
                        peer.quitFrom(this.groups[i]);
                    HostGroup.remove(this, this.groups[i]);
                    this.groups.RemoveAt(i--);
                }
            for (int i = 0; i < groups.Length; i++)
                if (!this.groups.Contains(groups[i]))
                {
                    if (peer != null)
                        peer.addInto(groups[i]);
                    HostGroup.add(this, groups[i]);
                    this.groups.Add(groups[i]);
                }
        }

        /// <summary>
        /// 提供ResHost的字符串表示形式
        /// </summary>
        /// <returns>ResHost的字符串表示形式</returns>
        public override string ToString()
        {
            return hostName + " : " + hostIPEP.ToString();
        }

        /// <summary>
        /// 用XElement对象生成ResHost对象
        /// </summary>
        /// <param name="xe">XElement对象</param>
        /// <returns>生成的ResHost对象</returns>
        public static ResHost FromXml(XElement xe)
        {
            try
            {
                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(xe.Attribute("address").Value), int.Parse(xe.Attribute("port").Value));
                return new ResHost(ipep, xe.Attribute("hostname").Value);
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 将ResHost对象转化为XElement对象
        /// </summary>
        /// <returns>转化成的XElement对象</returns>
        public XElement ToXml()
        {
            XElement root = new XElement("reshost");
            root.Add(new XAttribute("address", hostIPEP.Address.ToString()));
            root.Add(new XAttribute("port", hostIPEP.Port));
            root.Add(new XAttribute("hostname", hostName));
            return root;
        }
    }
}
