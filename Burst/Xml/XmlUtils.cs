using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Burst;

namespace Burst.Xml
{
    public static class XmlUtils
    {
        public static XElement XPathQuery(this XElement doc, string query)
        {
            XElement res = doc;
            string[] steps = query.Split('.');
            foreach (string s in steps)
            {
                string tagName = s;
                string attrName = null;
                string attrValue = null;
                if (s.IndexOf("[") != -1)
                {
                    tagName = s.Substring(0, s.IndexOf("["));
                    attrName = s.Substring(s.IndexOf("[") + 1, s.IndexOf("]") - s.IndexOf("[") - 1).Trim();
                    if (attrName.IndexOf("=") != -1)
                    {
                        attrValue = attrName.Split('=')[1].Trim();
                        attrName = attrName.Split('=')[0].Trim();
                    }
                }
                if (res.Elements(tagName).Count() == 0)
                    return null;
                foreach (XElement e in res.Elements(tagName))
                {
                    if (attrName == null)
                    {
                        res = e;
                        break;
                    }
                    if (e.Attribute(attrName) == null)
                        continue;
                    if (attrValue == null || e.Attribute(attrName).Value == attrValue)
                    {
                        res = e;
                        break;
                    }
                }
            }
            return res;
        }

        public static string XmlSerialize(object obj)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.Serialize(ms, obj);
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        public static object XmlDeserializeAs(byte[] data, Type type)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(type);
            MemoryStream ms = new MemoryStream();
            ms.Write(data, 0, data.Length);
            ms.Seek(0, 0);
            return serializer.Deserialize(ms);
        }
        public static object XmlDeserializeAs(string s, Type type)
        {
            return XmlDeserializeAs(Encoding.UTF8.GetBytes(s), type);
        }
        public static T XmlDeserializeAs<T>(byte[] data)
        {
            return (T)XmlDeserializeAs(data, typeof(T));
        }
        public static T XmlDeserializeAs<T>(string s)
        {
            return XmlDeserializeAs<T>(Encoding.UTF8.GetBytes(s));
        }
        public static T XmlDeserializeAs<T>(XElement doc)
        {
            return (T)new System.Xml.Serialization.XmlSerializer(typeof(T)).Deserialize(doc.CreateReader());
        }
    }
}
