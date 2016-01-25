using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;

namespace Burst.Data.Entity
{
    public static class Extension
    {
        public static DataTable AsDataTable<T>(this IEnumerable<DataEntityBase<T>> deb)
        {
            try
            {
                DataTable res = new DataTable();

                foreach (DataEntityBase<T> i in deb)
                {
                    List<Object> values = new List<Object>();
                    foreach (var fi in DataEntityBase<T>.AllFields)
                        values.Add(i.GetValue(fi.Attribute.DbFieldName));
                    res.Rows.Add(values.ToArray());
                }

                return res;
            }
            catch
            {
                return null;
            }
        }
    }
}
