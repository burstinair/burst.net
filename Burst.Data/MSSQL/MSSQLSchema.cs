using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.Common;
using Burst.Data.Schema;

namespace Burst.Data.MSSQL
{
    public class MSSQLSchema : DbSchema
    {
        protected override void LoadColumn(DataRow DataRow, ColumnInfo ColumnInfo)
        {
            ColumnInfo.DbType = DataRow[7] as string;
            ColumnInfo.Index = Convert.ToInt32(DataRow[4]);
            ColumnInfo.IsKey = false;
            ColumnInfo.IsNullable = DataRow[6] as string == "YES";
        }

        protected override void LoadTables(DbConnection Connection, Dictionary<string, TableInfo> Tables)
        {
            base.LoadTables(Connection, Tables);

            var dt = Connection.GetSchema("IndexColumns");
            foreach (DataRow dr in dt.Rows)
                if (Convert.ToInt32(dr[8]) == 56)
                    Tables[dr[5] as string].Columns[dr[6] as string].IsKey = true;
        }
    }
}
