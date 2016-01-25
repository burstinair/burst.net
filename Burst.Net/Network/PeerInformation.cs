using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Burst.Net
{
    /// <summary>
    /// 表示Peer的信息
    /// </summary>
    /// <remarks>
    /// 2010/09/19 Burst-最后一次修改
    /// </remarks>
    public class PeerInformation
    {
        /// <summary>
        /// Peer的IPEndPoint
        /// </summary>
        public IPEndPoint ipep;
        /// <summary>
        /// Peer是否已经被认可
        /// </summary>
        public bool isIn;
        /// <summary>
        /// Peer的ID
        /// </summary>
        public PeerID id;

        /// <summary>
        /// 初始化PeerInfomation
        /// </summary>
        /// <param name="ep">Peer的IPEndPoint</param>
        public PeerInformation(EndPoint ep)
        {
            this.ipep = ep as IPEndPoint;
            isIn = false;
        }
    }
}
