using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Burst.Net;

namespace Burst.Distribution
{
    /// <summary>
    /// 表示主机的信息
    /// </summary>
    /// <remarks>
    /// 2010/09/19 Burst-最后一次修改
    /// </remarks>
    [Serializable]
    public class HostInfo
    {
        public string SearchService;
        public string[] groups;
        public PeerID id;
 
        /// <summary>
        /// 初始化HostInfo
        /// </summary>
        public HostInfo()
        {
            this.id = HoldingServer.Self.id;
            this.SearchService = HoldingServer.Self.searchService;
            this.groups = HoldingServer.Self.groups.ToArray();
        }
    }
}
