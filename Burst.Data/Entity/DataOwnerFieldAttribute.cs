using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Burst.Data.CommandBuilder;

namespace Burst.Data.Entity
{
    /// <summary>
    /// 用于标识数据实体类型中的数据字段并设置基本信息。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class DataOwnerFieldAttribute : Attribute
    {
        public DataOwnerFieldAttribute(Type OwnedEntityType, string OwnedFieldName, bool AutoLoad)
        {
            this.GetMethod = Delegate.CreateDelegate(
                typeof(Func<Where, Page, Select, Transaction, Object[]>),
                OwnedEntityType, "Get")
                as Func<Where, Page, Select, Transaction, Object[]>;
            this.OwnedFieldName = OwnedFieldName;
            this.AutoLoad = AutoLoad;
        }

        public Func<Where, Page, Select, Transaction, Object[]> GetMethod;
        public string OwnedFieldName { get; set; }
        public bool AutoLoad { get; set; }
    }
}
