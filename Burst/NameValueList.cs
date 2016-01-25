using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Burst
{
    [Serializable]
    public class NameValueList : IEnumerable<NameValue>, ICollection<NameValue>, IXmlSerializable, ICloneable, ICustomTypeDescriptor
    {
        protected class KeyPair : IEquatable<string>
        {
            public bool ignoreCase;
            public string oriKey, lowerKey;
            public KeyPair(string oriKey, bool ignoreCase)
            {
                this.oriKey = oriKey;
                this.lowerKey = oriKey.ToLower();
                this.ignoreCase = ignoreCase;
            }
            public override int GetHashCode()
            {
                if (this.ignoreCase)
                    return this.lowerKey.GetHashCode();
                return this.oriKey.GetHashCode();
            }
            public override string ToString()
            {
                if (this.ignoreCase)
                    return this.lowerKey;
                return this.oriKey;
            }
            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                if (obj is KeyPair)
                {
                    if ((obj as KeyPair).ignoreCase || this.ignoreCase)
                        return this.lowerKey == (obj as KeyPair).lowerKey;
                    return this.oriKey == (obj as KeyPair).oriKey;
                }
                else if (obj is string)
                    return Equals(obj as string);
                return false;
            }
            public bool Equals(string obj)
            {
                if (this.ignoreCase)
                    return this.lowerKey == obj;
                return this.oriKey == obj;
            }
        }

        protected Dictionary<KeyPair, NameValue> dic;
        protected List<NameValue> data;

        public NameValueList()
        {
            this.ignoreCase = true;
            this.data = new List<NameValue>();
            this.dic = new Dictionary<KeyPair, NameValue>();
        }
        public NameValueList(bool IgnoreCase)
        {
            this.ignoreCase = IgnoreCase;
            this.data = new List<NameValue>();
            this.dic = new Dictionary<KeyPair, NameValue>();
        }
        public NameValueList(IEnumerable<NameValue> data)
        {
            this.ignoreCase = true;
            this.data = new List<NameValue>();
            this.dic = new Dictionary<KeyPair, NameValue>();
            foreach (NameValue i in data)
            {
                KeyPair key = GetKey(i.Name);
                if (!this.dic.ContainsKey(key))
                {
                    this.dic.Add(key, i);
                    this.data.Add(i);
                }
            }
        }
        public NameValueList(IEnumerable<KeyValuePair<string, object>> data)
        {
            this.ignoreCase = true;
            this.data = new List<NameValue>();
            this.dic = new Dictionary<KeyPair, NameValue>();
            foreach (KeyValuePair<string, object> i in data)
                Add(i.Key, i.Value);
        }
        public NameValueList(params NameValue[] data)
        {
            this.ignoreCase = true;
            this.data = new List<NameValue>();
            this.dic = new Dictionary<KeyPair, NameValue>();
            foreach (var i in data)
                Add(i);
        }

        protected KeyPair GetKey(string key)
        {
            return new KeyPair(key, this.ignoreCase);
        }

        public void Add(string name, object value)
        {
            if (isReadOnly)
                return;
            KeyPair key = GetKey(name);
            if (ContainsKey(name))
                dic[key].Value = value;
            else
            {
                NameValue nv = new NameValue(name, value);
                dic.Add(key, nv);
                data.Add(nv);
            }
        }
        public void Add(NameValue nv)
        {
            KeyPair key = GetKey(nv.Name);
            Debug.Assert(!this.dic.ContainsKey(key), "NameValueList.Add: Already Contains Key");
            this.dic.Add(key, nv);
            this.data.Add(nv);
        }
        public void Add(KeyValuePair<string, object> pair)
        {
            Add(pair.Key, pair.Value);
        }
        public void Insert(int index, NameValue nv)
        {
            if (isReadOnly)
                return;
            KeyPair key = GetKey(nv.Name);
            if (ContainsKey(nv.Name))
            {
                dic[key].Value = nv.Value;
                data.Insert(index, dic[key]);
            }
            else
            {
                dic.Add(key, nv);
                data.Insert(index, nv);
            }
        }
        public void Swap(int index1, int index2)
        {
            if (isReadOnly)
                return;
            if (index2 < index1)
            {
                int _t = index1;
                index1 = index2;
                index2 = _t;
            }
            if (index1 < 0 || index2 >= data.Count)
                return;
            NameValue t = data[index1];
            data[index1] = data[index2];
            data[index2] = t;
        }
        public void Swap(string name1, string name2)
        {
            if (ContainsKey(name1) && ContainsKey(name2))
                Swap(data.IndexOf(dic[GetKey(name1)]), data.IndexOf(dic[GetKey(name2)]));
        }
        public void Swap(string name, int index)
        {
            if (ContainsKey(name))
                Swap(index, data.IndexOf(dic[GetKey(name)]));
        }
        public bool Remove(string name)
        {
            if (isReadOnly)
                return false;
            KeyPair key = GetKey(name);
            if (ContainsKey(name))
            {
                data.Remove(dic[key]);
                dic.Remove(key);
                return true;
            }
            return false;
        }
        public bool Remove(NameValue nv)
        {
            if (isReadOnly)
                return false;
            if (ContainsKey(nv.Name))
                if (dic[GetKey(nv.Name)].Value == nv.Value)
                {
                    Remove(nv.Name);
                    return true;
                }
            return false;
        }
        public bool Remove(KeyValuePair<string, object> pair)
        {
            if (isReadOnly)
                return false;
            if (ContainsKey(pair.Key))
            {
                if (dic[GetKey(pair.Key)].Value == pair.Value)
                {
                    Remove(pair.Key);
                    return true;
                }
            }
            return false;
        }
        public bool RemoveAt(int index)
        {
            if (isReadOnly)
                return false;
            if (data.Count <= index || index < 0)
                return false;
            dic.Remove(GetKey(data[index].Name));
            data.RemoveAt(index);
            return true;
        }
        public bool ContainsKey(string key)
        {
            KeyPair pkey = GetKey(key);
            if (!dic.ContainsKey(pkey))
                return false;
            if (dic[pkey] == null)
                return false;
            return true;
        }
        public bool Contains(NameValue nv)
        {
            if (ContainsKey(nv.Name))
            {
                if (dic[GetKey(nv.Name)].Value == nv.Value)
                    return true;
            }
            return false;
        }
        public void CopyTo(NameValue[] nv, int index)
        {
            data.CopyTo(nv, index);
        }
        public void Clear()
        {
            if (isReadOnly)
                return;
            dic.Clear();
            data.Clear();
        }

        public NameValue[] ToArray()
        {
            return data.ToArray();
        }
        public List<NameValue> ToList()
        {
            return data.ToList();
        }
        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            foreach (NameValue i in data)
                res.Add(i.Name, i.Value);
            return res;
        }

        public object this[string name]
        {
            get
            {
                NameValue res = null;
                if (ContainsKey(name))
                    res = dic[GetKey(name)];
                if (res == null)
                    return null;
                return res.Value;
            }
            set
            {
                if (isReadOnly)
                    return;
                if (ContainsKey(name))
                    dic[GetKey(name)].Value = value;
                else
                    Add(name, value);
            }
        }
        public object this[int index]
        {
            get
            {
                if (data.Count <= index)
                    return null;
                if (data[index] == null)
                    return null;
                return data[index].Value;
            }
            set
            {
                if (isReadOnly)
                    return;
                while (data.Count <= index)
                    data.Add(null);
                if (data[index] != null)
                    data[index].Value = value;
                else
                {
                    NameValue nv = new NameValue(Guid.NewGuid().ToString(), value);
                    if (ContainsKey(nv.Name))
                    {
                        KeyPair key = GetKey(nv.Name);
                        data[data.IndexOf(dic[key])] = null;
                        dic[key] = nv;
                        data[index] = nv;
                    }
                    else
                    {
                        dic.Add(GetKey(nv.Name), nv);
                        data[index] = nv;
                    }
                }
            }
        }
        public T Get<T>(string name)
        {
            return (T)this[name];
        }
        public T Get<T>(int index)
        {
            return (T)this[index];
        }
        public ICollection<string> Keys
        {
            get
            {
                List<string> list = new List<string>();
                foreach (KeyPair kp in dic.Keys)
                    list.Add(kp.oriKey);
                return list;
            }
        }
        public ICollection<object> Values
        {
            get
            {
                List<object> res = new List<object>();
                foreach (NameValue nv in data)
                    res.Add(nv.Value);
                return res;
            }
        }
        public bool TryGetValue(string name, out object value)
        {
            if (ContainsKey(name))
            {
                value = dic[GetKey(name)].Value;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }
        public int Count
        {
            get { return data.Count; }
        }

        public string GetName(int index)
        {
            if (data.Count <= index)
                return null;
            if (data[index] == null)
                return null;
            return data[index].Name;
        }
        public string GetName(string name)
        {
            if (ContainsKey(name))
                return dic[GetKey(name)].Name;
            return null;
        }

        protected bool isReadOnly;
        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set { isReadOnly = value; }
        }
        protected bool ignoreCase;
        public bool IgnoreCase
        {
            get { return ignoreCase; }
            set
            {
                ignoreCase = value;
                if (value)
                    for (int i = 0; i < data.Count - 1; i++)
                    {
                        for (int j = i + 1; j < data.Count; j++)
                            if (data[i].Name.ToLower() == data[j].Name.ToLower())
                                RemoveAt(j--);
                    }
                Dictionary<KeyPair, NameValue> buf = dic;
                dic = new Dictionary<KeyPair, NameValue>();
                foreach (KeyValuePair<KeyPair, NameValue> p in buf)
                    dic.Add(GetKey(p.Key.oriKey), p.Value);
            }
        }

        IEnumerator<NameValue> IEnumerable<NameValue>.GetEnumerator()
        {
            return data.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            foreach (NameValue nv in data)
            {
                writer.WriteStartElement("item");
                new XmlSerializer(typeof(NameValue)).Serialize(writer, nv);
                writer.WriteEndElement();
            }
        }
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement || !reader.Read())
                return;
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                NameValue nv = new XmlSerializer(typeof(NameValue)).Deserialize(reader) as NameValue;
                Add(nv.Name, nv.Value);
                reader.ReadEndElement();
            }
            reader.ReadEndElement();
        }
        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        public object Clone()
        {
            NameValueList res = new NameValueList();
            foreach (NameValue i in this.data)
                res.Add(i.Name, i.Value);
            return res;
        }

        public AttributeCollection GetAttributes()
        {
            throw new NotImplementedException();
        }

        public string GetClassName()
        {
            return "Burst.NameValueList";
        }

        public string GetComponentName()
        {
            return "Burst.NameValueList";
        }

        public TypeConverter GetConverter()
        {
            throw new NotImplementedException();
        }

        public EventDescriptor GetDefaultEvent()
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            throw new NotImplementedException();
        }

        public object GetEditor(Type editorBaseType)
        {
            throw new NotImplementedException();
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            throw new NotImplementedException();
        }

        public EventDescriptorCollection GetEvents()
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            throw new NotImplementedException();
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            throw new NotImplementedException();
        }
    }
}
