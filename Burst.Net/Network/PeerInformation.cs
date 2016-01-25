using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Burst.Net
{
    /// <summary>
    /// ��ʾPeer����Ϣ
    /// </summary>
    /// <remarks>
    /// 2010/09/19 Burst-���һ���޸�
    /// </remarks>
    public class PeerInformation
    {
        /// <summary>
        /// Peer��IPEndPoint
        /// </summary>
        public IPEndPoint ipep;
        /// <summary>
        /// Peer�Ƿ��Ѿ����Ͽ�
        /// </summary>
        public bool isIn;
        /// <summary>
        /// Peer��ID
        /// </summary>
        public PeerID id;

        /// <summary>
        /// ��ʼ��PeerInfomation
        /// </summary>
        /// <param name="ep">Peer��IPEndPoint</param>
        public PeerInformation(EndPoint ep)
        {
            this.ipep = ep as IPEndPoint;
            isIn = false;
        }
    }
}
