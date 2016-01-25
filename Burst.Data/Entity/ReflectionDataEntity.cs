using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Burst.Data.Entity
{
    /// <summary>
    /// 采用反射的方法存取数据实体字段值的DataEntityBase类型的实现。
    /// </summary>
    /// <typeparam name="T">数据实体类型</typeparam>
    [Serializable]
    public class ReflectionDataEntity<T> : DataEntityBase<T>
    {
        private Dictionary<string, MemberInfo> _keyFieldMap;
        private MemberInfo GetFieldOrProperty(string key)
        {
            if (_keyFieldMap == null)
            {
                _keyFieldMap = new Dictionary<string, MemberInfo>();
                foreach (var fi in AllFields)
                    _keyFieldMap.Add(fi.Attribute.DbFieldName, fi.MemberInfo);
            }
            if (_keyFieldMap.ContainsKey(key))
                return _keyFieldMap[key];
            return null;
        }

        protected internal override object GetValue(string Key)
        {
            return GetFieldOrProperty(Key).GetValue(this);
        }

        protected internal override void SetValue(string Key, object Value)
        {
            GetFieldOrProperty(Key).SetValue(this, Value);
        }
    }
}
