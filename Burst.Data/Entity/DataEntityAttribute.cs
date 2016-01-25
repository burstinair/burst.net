using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Burst.Data.Entity
{
    /// <summary>
    /// 用于标识数据实体类型并设置类型的基本信息。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
    public class DataEntityAttribute : Attribute
    {
        /// <summary>
        /// 初始化DataEntityAttribute。
        /// </summary>
        /// <param name="TableName">所标识的实体类型对应的数据表名称</param>
        public DataEntityAttribute(string TableName, bool UseCache = false, Int64 TopLimit = 1000000000, Int64 StartIndex = 1000)
        {
            this.tableName = TableName;
            this.useCache = UseCache;
            this.topLimit = TopLimit;
            this.startIndex = StartIndex;
        }

        protected string tableName;
        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }

        protected bool useCache;
        public bool UseCache
        {
            get { return useCache; }
            set { useCache = value; }
        }

        protected Int64 topLimit;
        public Int64 TopLimit
        {
            get { return topLimit; }
            set { topLimit = value; }
        }

        protected Int64 startIndex;
        public Int64 StartIndex
        {
            get { return startIndex; }
            set { startIndex = value; }
        }
    }
}
