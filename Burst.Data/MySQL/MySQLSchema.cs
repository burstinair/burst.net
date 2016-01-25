using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.Common;
using Burst.Data.Schema;

namespace Burst.Data.MySQL
{
    public class MySQLSchema : DbSchema
    {
        protected override void LoadColumn(DataRow DataRow, ColumnInfo ColumnInfo)
        {
            ColumnInfo.DbType = DataRow[7] as string;
            ColumnInfo.Index = Convert.ToInt32(DataRow[4]);
            ColumnInfo.IsKey = DataRow[14] as string == "PRI";
            ColumnInfo.IsNullable = DataRow[6] as string == "YES";
        }
    }
}
