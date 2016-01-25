using System;
using System.Collections.Generic;
using System.Text;

namespace Burst.Xml
{
    public class XmlSerializer : ISerializer
    {
        public object Serialize(object obj)
        {
            return XmlUtils.XmlSerialize(obj);
        }

        public object Deserialize(object data, Type type)
        {
            if (data is byte[])
                return XmlUtils.XmlDeserializeAs(data as byte[], type);
            else
                return XmlUtils.XmlDeserializeAs(data.ToString(), type);
        }
    }
}
