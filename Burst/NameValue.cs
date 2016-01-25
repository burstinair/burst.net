using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Burst.Json;

namespace Burst
{
    [Serializable]
    public class NameValue : IXmlSerializable, IJsonObject, ICloneable
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public NameValue()
        {
            this.Name = null;
            this.Value = null;
        }
        public NameValue(string Name, object Value)
        {
            this.Name = Name;
            this.Value = Value;
        }

        string IJsonSerializeObject.SerializeToJsonString()
        {
            return string.Format(@"{{""{0}"":{1}}}", Name, JsonUtils.Serialize(Value));
        }

        void IFieldWritable.SetFieldValue(string fieldName, object value)
        {
            this.Name = fieldName;
            this.Value = value;
        }

        Type IFieldWritable.GetFieldType(string fieldName)
        {
            return typeof(object);
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            XmlSerializer ss = new XmlSerializer(typeof(string));
            writer.WriteStartElement("name");
            ss.Serialize(writer, Name);
            writer.WriteEndElement();
            writer.WriteStartElement("type");
            ss.Serialize(writer, Value.GetType().AssemblyQualifiedName);
            writer.WriteEndElement();
            writer.WriteStartElement("value");
            new XmlSerializer(Value.GetType()).Serialize(writer, Value);
            writer.WriteEndElement();
        }
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement || !reader.Read())
                return;
            XmlSerializer ss = new XmlSerializer(typeof(string));
            reader.ReadStartElement("name");
            Name = ss.Deserialize(reader) as string;
            reader.ReadEndElement();
            reader.ReadStartElement("type");
            XmlSerializer vs = new XmlSerializer(Type.GetType(ss.Deserialize(reader) as string));
            reader.ReadEndElement();
            reader.ReadStartElement("value");
            Value = vs.Deserialize(reader);
            reader.ReadEndElement();
            reader.ReadEndElement();
        }
        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        public Object Clone()
        {
            return new NameValue(this.Name, this.Value);
        }
    }
}
