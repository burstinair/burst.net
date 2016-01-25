using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Burst.Data.Schema
{
    public class TableInfo : ICloneable
    {
        public string Name { get; set; }
        public Dictionary<string, ColumnInfo> Columns { get; set; }

        public TableInfo()
        {
            this.Columns = new Dictionary<string, ColumnInfo>();
        }

        #region ICloneable 成员

        public object Clone()
        {
            var res = new TableInfo();
            res.Name = this.Name;
            res.Columns = new Dictionary<string, ColumnInfo>();
            foreach (var kvp in this.Columns)
                res.Columns.Add(kvp.Key, kvp.Value.Clone() as ColumnInfo);
            return res;
        }

        #endregion
    }
}
