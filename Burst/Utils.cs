using System;
using System.Data;
using System.Reflection;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;
using System.Xml;
using System.Net;
using Burst.Crypt;
using Burst.Json;
using Burst.Xml;

namespace Burst
{
    public enum SerializeType
    {
        None, String, Json, Xml, Binary
    }

    public static class Utils
    {
        public static string EncodeCRLF(string source)
        {
            if (source == null)
                return null;
            StringBuilder res = new StringBuilder();
            int p = 0;
            while (p < source.Length)
            {
                switch (source[p])
                {
                    case '\\':
                        res.Append("\\\\");
                        p++;
                        break;
                    case '\n':
                        res.Append("\\n");
                        if (p + 1 < source.Length)
                            if (source[p + 1] == '\r')
                            {
                                p += 2;
                                continue;
                            }
                        p++;
                        break;
                    case '\r':
                        res.Append("\\n");
                        if (p + 1 < source.Length)
                            if (source[p + 1] == '\n')
                            {
                                p += 2;
                                continue;
                            }
                        p++;
                        break;
                    default:
                        res.Append(source[p]);
                        p++;
                        break;
                }
            }
            return res.ToString();
        }
        public static string DecodeCRLF(string source)
        {
            if (source == null)
                return null;
            StringBuilder res = new StringBuilder();
            int pre = 0;
            int p = source.IndexOf('\\');
            while (p != -1)
            {
                res.Append(source.Substring(pre, p - pre));
                switch (source[p + 1])
                {
                    case 'n':
                        res.Append("\r\n");
                        break;
                    case '\\':
                        res.Append('\\');
                        break;
                }
                pre = p + 2;
                p = source.IndexOf('\\', p + 2);
            }
            if (pre < source.Length)
                res.Append(source.Substring(pre, source.Length - pre));
            return res.ToString();
        }

        public static string SerializeToString(object data, SerializeType type)
        {
            if (data is Encoding)
                return (data as Encoding).BodyName;
            else if (data is Uri)
                return (data as Uri).AbsolutePath;
            else if (data is byte[])
                return Settings.Encoding.GetString(data as byte[]);
            else if (data is Stream)
                return new StreamReader(data as Stream).ReadToEnd();
            else
            {
                switch (type)
                {
                    case SerializeType.Json:
                        return JsonUtils.Serialize(data);
                    case SerializeType.Xml:
                        return XmlUtils.XmlSerialize(data);
                    default:
                        return data.ToString();
                }
            }
        }
        public static byte[] SerializeToBinary(object data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, data);
            return ms.ToArray();
        }
        public static void SerializeToStream(object data, Stream s)
        {
            new StreamWriter(s).Write(SerializeToBinary(data));
        }
        public static object Serialize(object data)
        {
            return SerializeToString(data, SerializeType.None);
        }
        public static object Serialize(object data, SerializeType type)
        {
            if (data is ICustomSerializeObject)
                return (data as ICustomSerializeObject).Serialize();
            if (type == SerializeType.Binary)
                return SerializeToBinary(data);
            return SerializeToString(data, type);
        }

        public static T DeserializeAs<T>(object oridata)
        {
            return (T)Utils.DeserializeAs(oridata, typeof(T), SerializeType.None);
        }
        public static T DeserializeAs<T>(object oridata, SerializeType type)
        {
            return (T)Utils.DeserializeAs(oridata, typeof(T), type);
        }
        public static object DeserializeAs(object oridata, Type type, SerializeType stype)
        {
            if (oridata.GetType() == type && stype == SerializeType.None)
                return oridata;
            if (!type.IsValueType && type.Implements(typeof(ICustomSerializeObject)))
            {
                Object res = Activator.CreateInstance(type);
                (res as ICustomSerializeObject).Deserialize(oridata);
                return res;
            }
            if (stype == SerializeType.Binary)
            {
                byte[] bdata;
                if (oridata is string)
                    bdata = Settings.Encoding.GetBytes(oridata as string);
                else if (oridata is byte[])
                    bdata = oridata as byte[];
                else if (oridata is Stream)
                    bdata = new BinaryReader(oridata as Stream).ReadBytes(int.MaxValue);
                else
                    return null;
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(bdata);
                return bf.Deserialize(ms);
            }
            string data;
            if (oridata is string)
                data = oridata as string;
            else if (oridata is byte[])
                data = Settings.Encoding.GetString(oridata as byte[]);
            else if (oridata is Stream)
                data = new StreamReader(oridata as Stream).ReadToEnd();
            else
                data = oridata.ToString();
            try
            {
                if (stype == SerializeType.Json)
                    return JsonUtils.ParseAs(data, type);
                else if (stype == SerializeType.Xml)
                    return XmlUtils.XmlDeserializeAs(data, type);
                else
                {
                    if (type.Equals(typeof(string)))
                        return data;
                    else if (type.IsEnum)
                        return Enum.Parse(type, data, true);
                    else if (type.Equals(typeof(DateTimeOffset)))
                        return DateTimeOffset.Parse(data);
                    else if (type.Equals(typeof(TimeSpan)))
                        return TimeSpan.Parse(data);
                    else if (type.IsValueType)
                        return Convert.ChangeType(data, type);
                    else if (type.Equals(typeof(IPAddress)))
                        return IPAddress.Parse(data);
                    else if (type.Equals(typeof(IPEndPoint)))
                    {
                        string[] ops = data.Split(':');
                        return new IPEndPoint(IPAddress.Parse(ops[0].Trim()), int.Parse(ops[1].Trim()));
                    }
                    else if (type.Equals(typeof(XElement)))
                        return XElement.Parse(data);
                    else if (type.Equals(typeof(XDocument)))
                        return XDocument.Parse(data);
                    else if (type.Equals(typeof(XmlDocument)))
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(data);
                        return doc;
                    }
                    else if (type.Equals(typeof(Encoding)))
                        return Encoding.GetEncoding(data);
                    else if (type.Equals(typeof(Uri)))
                        return new Uri(data);
                    else
                        return null;
                }
            }
            catch { }
            if (type.IsValueType)
                return TypeUtils.GetDefaultValue(type);
            return null;
        }

        public static DateTime GetDateTimeInDefaultValue(string str)
        {
            try
            {
                return DateTime.Parse(str);
            }
            catch { }
            return default(DateTime);
        }
        public static string TrimTimeString(DateTime datetime)
        {
            if (datetime == default(DateTime))
                return "";
            return datetime.ToString();
        }
        public static string TrimTimeString(DateTime datetime, string format)
        {
            if (datetime == default(DateTime))
                return "";
            return datetime.ToString(format);
        }
        public static string TimeID(DateTime time)
        {
            return CryptUtils.Encrypt(string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond, new Random().Next()), CryptType.MD5);
        }
    }
}
