using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Burst
{
    public class BinarySerializer : ISerializer
    {
        public object Serialize(object oriobj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, oriobj);
            return ms.ToArray();
        }

        public object Deserialize(object data, Type type)
        {
            byte[] bdata;
            if (data is string)
                bdata = Settings.Encoding.GetBytes(data as string);
            else if (data is byte[])
                bdata = data as byte[];
            else if (data is Stream)
                bdata = new BinaryReader(data as Stream).ReadBytes(int.MaxValue);
            else
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(bdata);
            return bf.Deserialize(ms);
        }
    }
}
