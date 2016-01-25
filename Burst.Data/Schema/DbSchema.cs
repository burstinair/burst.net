using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace Burst.Data.Schema
{
    public class DbSchema
    {
        private DbProvider Provider;

        public DbSchema()
        { }
        public DbSchema(DbProvider Provider)
        {
            this.Provider = Provider;
            LoadSchema();
        }
        public DbSchema(DbConnection Connection)
        {
            LoadSchema(Connection);
        }

        public void LoadSchema(DbConnection Connection)
        {
            _dic_type = new Dictionary<string, Type>();
            LoadDataTypes(Connection, _dic_type);
            _dic_table = new Dictionary<string, TableInfo>();
            LoadTables(Connection, _dic_table);
        }
        protected virtual bool LoadSchema()
        {
            try
            {
                using (var conn = Provider.CreateConnection())
                {
                    conn.Open();
                    LoadSchema(conn);
                }
                return true;
            }
            catch { }
            return false;
        }
        private Dictionary<string, Type> _dic_type;
        public Type TranslateType(string DbType)
        {
            if (_dic_type == null)
                ReloadSchema();
            if (_dic_type.ContainsKey(DbType))
                return _dic_type[DbType];
            else
                return null;
        }
        public Type[] AllTypes
        {
            get
            {
                if (_dic_type == null)
                    ReloadSchema();
                return _dic_type.Values.ToArray();
            }
        }
        private Dictionary<string, TableInfo> _dic_table;
        public TableInfo GetTableInfo(string TableName)
        {
            if (_dic_table == null)
                ReloadSchema();
            if (_dic_table.ContainsKey(TableName))
                return _dic_table[TableName].Clone() as TableInfo;
            else
                return null;
        }
        public TableInfo[] AllTables
        {
            get
            {
                if (_dic_table == null)
                    ReloadSchema();
                var res = new List<TableInfo>();
                foreach (var t in _dic_table.Values)
                    res.Add(t.Clone() as TableInfo);
                return res.ToArray();
            }
        }
        protected virtual void LoadDataTypes(DbConnection Connection, Dictionary<string, Type> Types)
        {
            var dt = Connection.GetSchema("DataTypes");
            foreach (DataRow r in dt.Rows)
                Types.Add(r[0] as string, Type.GetType(r[5] as string));
        }
        protected virtual int TableNameColumnIndex
        {
            get { return 2; }
        }
        protected virtual void LoadTables(DbConnection Connection, Dictionary<string, TableInfo> Tables)
        {
            var dt = Connection.GetSchema("Tables");
            foreach (DataRow r in dt.Rows)
            {
                var info = new TableInfo();
                info.Name = r[TableNameColumnIndex] as string;
                Tables.Add(info.Name, info);
            }
            dt = Connection.GetSchema("Columns");
            foreach (DataRow r in dt.Rows)
            {
                var cinfo = new ColumnInfo();
                cinfo.TableName = r[2] as string;
                cinfo.Name = r[3] as string;
                LoadColumn(r, cinfo);
                cinfo.Type = TranslateType(cinfo.DbType);
                Tables[cinfo.TableName].Columns.Add(cinfo.Name, cinfo);
            }
        }
        protected virtual void LoadColumn(DataRow DataRow, ColumnInfo ColumnInfo)
        {
            throw new NotImplementedException();
        }
        public void LoadSchema(DbProvider Provider)
        {
            this.Provider = Provider;
            LoadSchema();
        }
        public void ReloadSchema()
        {
            if (this.Provider == null)
                throw new DbSchemaException("未设置DbProvider。", null);
            else
                LoadSchema();
        }
    }
}
