using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Burst.Data.Utils
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
    public class DbConfigurationAttribute : Attribute
    {
        public DbConfigurationAttribute()
        {
            this.tableName = "configuration";
            this.keyFieldName = "key";
            this.typeFieldName = "type";
            this.valueFieldName = "value";
            this.sType = SerializeType.Xml;
        }
        public DbConfigurationAttribute(string TableName, SerializeType sType)
        {
            this.tableName = TableName;
            this.keyFieldName = "key";
            this.typeFieldName = "type";
            this.valueFieldName = "value";
            this.sType = sType;
        }
        public DbConfigurationAttribute(string TableName, string KeyFieldName, string TypeFieldname, string ValueFieldName, SerializeType sType)
        {
            this.tableName = TableName;
            this.keyFieldName = KeyFieldName;
            this.typeFieldName = TypeFieldname;
            this.valueFieldName = ValueFieldName;
            this.sType = sType;
        }

        protected string tableName;
        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }

        protected string keyFieldName;
        public string KeyFieldName
        {
            get { return keyFieldName; }
            set { keyFieldName = value; }
        }

        protected string typeFieldName;
        public string TypeFieldName
        {
            get { return typeFieldName; }
            set { typeFieldName = value; }
        }

        protected string valueFieldName;
        public string ValueFieldName
        {
            get { return valueFieldName; }
            set { valueFieldName = value; }
        }

        protected SerializeType sType;
        public SerializeType SerializeType
        {
            get { return sType; }
            set { sType = value; }
        }
    }
}
