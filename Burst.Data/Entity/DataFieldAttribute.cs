using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Burst.Data.Entity
{
    /// <summary>
    /// 用于标识数据实体类型中的数据字段并设置基本信息。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class DataFieldAttribute : Attribute
    {
        /// <summary>
        /// 初始化DataFieldAttribute。
        /// </summary>
        public DataFieldAttribute(string DbFieldName, SerializeType SerializeType = Burst.SerializeType.None)
        {
            this.DbFieldName = DbFieldName;
            this.SerializeType = SerializeType;
            this.DisplayName = null;
        }
        /// <summary>
        /// 初始化DataFieldAttribute。
        /// </summary>
        public DataFieldAttribute(string DbFieldName, string DisplayName, SerializeType SerializeType = Burst.SerializeType.None)
        {
            this.DbFieldName = DbFieldName;
            this.SerializeType = SerializeType;
            this.DisplayName = DisplayName;
        }

        public string DbFieldName { get; set; }
        public SerializeType SerializeType { get; set; }
        public string DisplayName { get; set; }
    }
}
