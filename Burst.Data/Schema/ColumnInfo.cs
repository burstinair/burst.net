using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Burst.Data.Schema
{
    public class ColumnInfo : ICloneable
    {
        public string TableName { get; set; }
        public string Name { get; set; }
        public string DbType { get; set; }
        public Type Type { get; set; }
        public SerializeType SerializeType { get; set; }
        public bool IsKey { get; set; }
        public bool IsNullable { get; set; }
        public int Index { get; set; }

        #region ICloneable 成员

        public object Clone()
        {
            var res = new ColumnInfo();
            res.TableName = this.TableName;
            res.Name = this.Name;
            res.DbType = this.DbType;
            res.Type = this.Type;
            res.SerializeType = this.SerializeType;
            res.IsKey = this.IsKey;
            res.IsNullable = this.IsNullable;
            res.Index = this.Index;
            return res;
        }

        #endregion
    }
}
