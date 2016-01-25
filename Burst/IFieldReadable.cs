using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Burst
{
    public interface IFieldReadable
    {
        object GetFieldValue(string fieldName);

        /// <summary>
        /// 获取指定字段的类型。
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <returns>该字段的类型</returns>
        Type GetFieldType(string fieldName);
    }
}
