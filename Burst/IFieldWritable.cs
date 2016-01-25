using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Burst
{
    public interface IFieldWritable
    {
        /// <summary>
        /// 给对象的指定字段赋值。
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="value">值</param>
        void SetFieldValue(string fieldName, object value);

        /// <summary>
        /// 获取指定字段的类型。
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <returns>该字段的类型</returns>
        Type GetFieldType(string fieldName);
    }
}
