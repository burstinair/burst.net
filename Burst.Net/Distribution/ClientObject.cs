using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using Burst.Net;

namespace Burst.Distribution
{
    /// <summary>
    /// 分布式系统中存储非管理主机的特有属性
    /// </summary>
    /// <remarks>
    /// 2010/09/19 Burst-最后一次修改
    /// </remarks>
    public class ClientObject
    {
        public IPAddress remote = null;
        public Peer server;
    }
}
