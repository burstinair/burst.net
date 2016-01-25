using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

using Burst.Data.Schema;

namespace Burst.Data.SQLite
{
    public class SQLiteSchema : DbSchema
    {
        protected override void LoadColumn(DataRow DataRow, ColumnInfo ColumnInfo)
        {
            ColumnInfo.DbType = DataRow[11] as string;
            ColumnInfo.Index = Convert.ToInt32(DataRow[6]);
            ColumnInfo.IsKey = Convert.ToBoolean(DataRow[27]);
            ColumnInfo.IsNullable = Convert.ToBoolean(DataRow[10]);
        }
    }
}
