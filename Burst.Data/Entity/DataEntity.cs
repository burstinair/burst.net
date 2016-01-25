using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;

namespace Burst.Data.Entity
{
    /// <summary>
    /// 采用Hashtable存取数据实体字段值的DataEntityBase类型的实现。
    /// </summary>
    /// <typeparam name="T">数据实体类型</typeparam>
    [Serializable]
    public class DataEntity<T> : DataEntityBase<T>
    {
        private Hashtable __baseData;
        private Hashtable _baseData
        {
            get
            {
                if (__baseData == null)
                {
                    try
                    {
                        __baseData = new Hashtable();
                        foreach (var fi in AllFields)
                            __baseData.Add(fi.Attribute.DbFieldName, null);
                    }
                    catch { }
                }
                return __baseData;
            }
        }

        /// <summary>
        /// 用于获取数据实体对象中字段的值。
        /// </summary>
        /// <param name="Key">字段名</param>
        /// <returns>字段的值</returns>
        protected internal override object GetValue(string Key)
        {
            return _baseData[Key];
        }

        /// <summary>
        /// 用于设置数据实体对象中字段的值。
        /// </summary>
        /// <param name="Key">字段名</param>
        /// <param name="Value">字段的值</param>
        protected internal override void SetValue(string Key, object Value)
        {
            _baseData[Key] = Value;
        }
    }
}
