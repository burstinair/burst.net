using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Reflection;
using Burst;
using Burst.Data.Schema;
using Burst.Data.CommandBuilder;
using Burst.Json;

namespace Burst.Data.Entity
{
    /// <summary>
    /// 提供数据实体类型基本方法的支持，通过泛型获取数据实体类型完成字段结构的初始化。
    /// </summary>
    /// <typeparam name="T">数据实体类型</typeparam>
    [DataEntity(""), Serializable]
    public abstract class DataEntityBase<T> : IDataEntity
    {
        /// <summary>
        /// 虚方法，用于数据实体对象字段值的获取，请勿直接调用此方法。
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        protected internal abstract object GetValue(string Key);
        /// <summary>
        /// 虚方法，用于数据实体对象字段值的设置，请勿直接调用此方法。
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        protected internal abstract void SetValue(string Key, object Value);

        /*
        public virtual void InitializeOwner(Transaction trans, params string[] fields)
        {
            bool all = false;
            if (fields.Length == 0)
                all = true;
            foreach (MemberInfo mi in OwnerFields)
            {
                if (all || mi.Name.In(fields))
                {
                    DataOwnerFieldAttribute a = mi.GetAttribute<DataOwnerFieldAttribute>();
                    SetValue(mi.Name, a.GetMethod(
                        new Where(a.OwnedFieldName + "=@0", Key), null, null, trans));
                }
            }
        }
        protected virtual void InitializeAutoLoadFields(Transaction trans)
        {
            List<string> autoloadfields = new List<string>();
            foreach (MemberInfo mi in OwnerFields)
            {
                DataOwnerFieldAttribute a = mi.GetAttribute<DataOwnerFieldAttribute>();
                if (a.AutoLoad)
                    autoloadfields.Add(mi.Name);
            }
            if (autoloadfields.Count > 0)
                InitializeOwner(trans, autoloadfields.ToArray());
        }
        */
        protected virtual void InitializeOri(NameValueList nv, Transaction trans)
        {
            foreach (var fi in AllFields)
            {
                if (nv.ContainsKey(fi.Attribute.DbFieldName))
                    SetValue(fi.Attribute.DbFieldName, nv[fi.Attribute.DbFieldName]);
            }
            //InitializeAutoLoadFields(trans);
        }
        protected virtual bool Initialize(DbDataReader reader, Transaction trans)
        {
            bool containsAllFields = true;
            foreach (var fi in AllFields)
            {
                int ordinal = reader.GetOrdinal(fi.Attribute.DbFieldName);
                if (ordinal >= 0)
                    SetValue(fi.Attribute.DbFieldName, Burst.Utils.DeserializeAs(reader.GetValue(ordinal), fi.Type, fi.Attribute.SerializeType));
                else
                    containsAllFields = false;
            }
            //InitializeAutoLoadFields(trans);
            return containsAllFields;
        }
        public static T GetSingleByCommand(Command cmd, Transaction trans)
        {
            T item = default(T);
            cmd.ExecuteReader(new DBReaderDelegate((DbDataReader reader) =>
            {
                if (item == null)
                {
                    item = Activator.CreateInstance<T>();
                    if ((item as DataEntityBase<T>).Initialize(reader, trans))
                        _cache.SaveCache((item as DataEntityBase<T>).Key, item);
                }
                return false;
            }), trans);
            return item;
        }
        public static T[] GetByCommand(Command cmd, Transaction trans)
        {
            List<T> res = new List<T>();
            cmd.ExecuteReader(new DBReaderDelegate((DbDataReader reader) =>
            {
                var item = Activator.CreateInstance<T>();
                if ((item as DataEntityBase<T>).Initialize(reader, trans))
                    _cache.SaveCache((item as DataEntityBase<T>).Key, item);
                res.Add(item);
                return true;
            }), trans);
            return res.ToArray();
        }

        #region Execute

        public static T GetByKey(Object key)
        {
            return GetByKey(key, null, null, true);
        }
        public static T GetByKey(Object key, Transaction trans)
        {
            return GetByKey(key, null, trans, true);
        }
        public static T GetByKey(Object key, Select select, Transaction trans, bool useCache)
        {
            if (KeyField == null)
                return default(T);
            if (UseCache && useCache)
            {
                T res = _cache.GetCache(key);
                if (res != null)
                    return res;
            }
            Command cmd = DbProvider.CreateCommand("select");
            Builder.AppendSelect(cmd, select, DbProvider.Adapter);
            cmd.AppendFormat(" from {0} where {1}=@0WhereKey", EnsuredTableName, KeyField.EnsuredName);
            cmd.AddParameter("@0WhereKey", key);
            return GetSingleByCommand(cmd, trans);
        }
        public static T GetByKeyForFormView(Object key)
        {
            T res = GetByKey(key, null, null, true);
            if (res == null)
                res = Activator.CreateInstance<T>();
            return res;
        }
        public static T GetSingle(Where where, Select select, Transaction trans)
        {
            Command cmd = DbProvider.CreateCommand("select");
            Builder.AppendSelect(cmd, select, DbProvider.Adapter);
            cmd.AppendFormat(" from {0}", EnsuredTableName);
            Builder.AppendWhereByEnsuredKey(cmd, where, KeyField.EnsuredName, DbProvider.Adapter);
            Builder.AppendPage(cmd, KeyField.Attribute.DbFieldName, TableName, new Page(0, 1), where, DbProvider.Adapter);
            return GetSingleByCommand(cmd, trans);
        }
        public static T GetSingle(string where, Transaction trans)
        {
            return GetSingle(new Where(where), null, trans);
        }
        public static T[] Get(Where where, Page page, Select select, Transaction trans)
        {
            Command cmd = DbProvider.CreateCommand("select");
            Builder.AppendSelect(cmd, select, DbProvider.Adapter);
            cmd.AppendFormat(" from {0}", EnsuredTableName);
            List<Object> pms = new List<Object>();
            Builder.AppendWhereByEnsuredKey(cmd, where, KeyField.EnsuredName, DbProvider.Adapter);
            Builder.AppendPage(cmd, KeyField.Attribute.DbFieldName, TableName, page, where, DbProvider.Adapter);
            return GetByCommand(cmd, trans);
        }
        public static T[] Get(string where, Transaction trans)
        {
            return Get(new Where(where), null, null, trans);
        }
        public static T[] Page(Where where, Select select, Transaction trans, int maximumRows, int startRowIndex)
        {
            return Get(where, new Page(startRowIndex, maximumRows), select, trans);
        }
        public static T[] Page(int maximumRows, int startRowIndex)
        {
            return Get(null, new Page(startRowIndex, maximumRows), null, null);
        }
        public static object GetMax(string fieldName, Transaction trans)
        {
            Command cmd = DbProvider.CreateCommand(string.Format(
                "select {0} from {1}", DbProvider.Adapter.EnsureIdentifier(fieldName), EnsuredTableName));
            Where where = new Where();
            where.AddOrder(fieldName, OrderType.Desc);
            Builder.AppendWhereByEnsuredKey(cmd, where, KeyField.EnsuredName, DbProvider.Adapter);
            Builder.AppendPage(cmd, KeyField.Attribute.DbFieldName, TableName, new Page(0, 1), where, DbProvider.Adapter);
            return Burst.Utils.DeserializeAs(
                cmd.ExecuteScalar(trans),
                _schema.Columns[fieldName].Type,
                _schema.Columns[fieldName].SerializeType
            );
        }
        public static object GetMin(string fieldName, Transaction trans)
        {
            Command cmd = DbProvider.CreateCommand(string.Format(
                "select {0} from {1}", DbProvider.Adapter.EnsureIdentifier(fieldName), EnsuredTableName));
            Where where = new Where();
            where.AddOrder(fieldName, OrderType.Asc);
            Builder.AppendWhereByEnsuredKey(cmd, where, KeyField.EnsuredName, DbProvider.Adapter);
            Builder.AppendPage(cmd, KeyField.Attribute.DbFieldName, TableName, new Page(0, 1), where, DbProvider.Adapter);
            return Burst.Utils.DeserializeAs(
                cmd.ExecuteScalar(trans),
                _schema.Columns[fieldName].Type,
                _schema.Columns[fieldName].SerializeType
            );
        }
        public static int GetCount(Where where, Select select, Transaction trans)
        {
            Command cmd = DbProvider.CreateCommand(string.Format("select count(*) from {0}", EnsuredTableName));
            Builder.AppendWhereByEnsuredKey(cmd, where, KeyField.EnsuredName, DbProvider.Adapter);
            object res = cmd.ExecuteScalar(trans);
            return Convert.ToInt32(res);
        }
        public static int GetCount()
        {
            return GetCount(null, null, null);
        }
        public static SearchResult<T> Search(Page page, Where where, Select select, Transaction trans)
        {
            SearchResult<T> res = new SearchResult<T>();
            if (page != null)
            {
                res.StartPos = page.StartPos;
                res.Count = page.Count;
            }

            Transaction _trans = trans;
            if (_trans == null)
                _trans = new Transaction(true);

            res.Data = Get(where, page, select, trans);
            res.Total = GetCount(where, select, trans);

            if (trans == null)
                _trans.Commit();

            return res;
        }
        public static bool CheckExist(Where where, Transaction trans)
        {
            Command cmd = DbProvider.CreateCommand(string.Format("select * from {0}", EnsuredTableName));
            Builder.AppendWhereByEnsuredKey(cmd, where, KeyField.EnsuredName, DbProvider.Adapter);
            NameValueList[] res = cmd.Execute(trans);
            if (res != null)
                if (res.Length > 0)
                    return true;
            return false;
        }
        public static Int64 GetNextIndex(Transaction trans)
        {
            if (IndexField != null)
            {
                try
                {
                    var fn = IndexField.Attribute.DbFieldName;
                    var kn = KeyField.Attribute.DbFieldName;
                    Command cmd = DbProvider.CreateCommand(string.Format("select {0} from {1}", IndexField.EnsuredName, EnsuredTableName));
                    Where where = new Compare(CompareType.LessThan, fn, TopLimit).ToWhere();
                    where.AddOrder(fn, OrderType.Desc);
                    Builder.AppendWhere(cmd, where, kn, DbProvider.Adapter);
                    Builder.AppendPage(cmd, kn, TableName, new Page(0, 1), where, DbProvider.Adapter);
                    return Convert.ToInt64(
                        cmd.ExecuteScalar(trans)
                    ) + 1;
                }
                catch { return StartIndex; }
            }
            else
                throw new IndexFieldException("没有Index字段。", null);
        }
        public bool Top
        {
            get
            {
                if (IndexField != null)
                {
                    try
                    {
                        return Convert.ToInt64(GetValue(IndexField.Attribute.DbFieldName)) >= TopLimit;
                    }
                    catch (Exception e)
                    {
                        throw new IndexFieldException("将Index字段值转换为Int64时发生错误。", e);
                    }
                }
                else
                    throw new IndexFieldException("没有Index字段。", null);
            }
        }

        #endregion

        protected static NameValue GetFieldValue(DataEntityFieldInfo fi, InsertType iops, object v)
        {
            if (fi.Key)
            {
                if (iops == InsertType.New)
                {
                    v = null;
                }
                else if (v != null && iops == InsertType.Last_Insert_ID)
                {
                    if (v.GetType().IsValueType)
                        if (v.Equals(TypeUtils.GetDefaultValue(v.GetType())) || v is string && (v as string) == "")
                            v = DbProvider.Adapter.LastInsertId();
                }
            }
            if (v != null && fi.Attribute.SerializeType != SerializeType.None)
                v = Burst.Utils.Serialize(v, fi.Attribute.SerializeType);
            return new NameValue("@" + fi.Attribute.DbFieldName, v);
        }
        protected virtual NameValueList AsNameValueList(InsertType iops)
        {
            NameValueList res = new NameValueList();
            for (int i = 0; i < AllFields.Length; i++)
            {
                var fi = AllFields[i];
                if (iops == InsertType.New && fi.Key)
                    continue;
                NameValue cnv = GetFieldValue(fi, iops, GetValue(fi.Attribute.DbFieldName));
                if (cnv != null)
                    res.Add(cnv);
            }
            return res;
        }
        protected virtual DataTable AsDataTable
        {
            get
            {
                return new DataEntityBase<T>[] { this }.AsDataTable<T>();
            }
        }

        #region ExecuteNonQuery

        public virtual Command GetDeleteCommand()
        {
            if (KeyField == null)
                return null;
            string cmd = string.Format("delete from {0} where {1}=@0", EnsuredTableName, KeyField.EnsuredName);
            return DbProvider.CreateCommand(cmd, Key);
        }
        public int Delete(Transaction trans)
        {
            return GetDeleteCommand().ExecuteNonQuery(trans);
        }
        public static Command GetDeleteCommand(Where where)
        {
            Command cmd = DbProvider.CreateCommand(string.Format("delete from {0}", EnsuredTableName));
            Builder.AppendWhereByEnsuredKey(cmd, where, KeyField.EnsuredName, DbProvider.Adapter);
            return cmd;
        }
        public static int Delete(Where where, Transaction trans)
        {
            return GetDeleteCommand(where).ExecuteNonQuery(trans);
        }
        public virtual Command GetInsertCommand(InsertType iops)
        {
            StringBuilder ss = new StringBuilder(), vs = new StringBuilder();
            foreach (var fi in AllFields)
            {
                if (fi.Key && iops == InsertType.New)
                    continue;
                ss.AppendFormat("{0},", fi.EnsuredName);
                vs.AppendFormat("@{0},", fi.Attribute.DbFieldName);
            }
            if (ss.Length > 0)
                ss.Remove(ss.Length - 1, 1);
            if (vs.Length > 0)
                vs.Remove(vs.Length - 1, 1);
            string cmd = string.Format("insert into {0} ({1}) values ({2})", EnsuredTableName, ss, vs);
            return DbProvider.CreateCommand(cmd, this.AsNameValueList(iops));
        }
        public int Insert(InsertType iops, Transaction trans)
        {
            Transaction _trans = trans;
            if (trans == null)
                _trans = new Transaction(true);
            int res = GetInsertCommand(iops).ExecuteNonQuery(_trans);
            if (res > 0)
                Key = Burst.Utils.DeserializeAs(LastInsertKey(_trans), KeyField.Type, SerializeType.None);
            if (trans == null)
                _trans.Commit();
            return res;
        }
        public static Command GetInsertCommand(InsertType iops, IEnumerable<NameValue> pms)
        {
            if (pms.Count() == 0)
                return null;
            StringBuilder ss = new StringBuilder(), vs = new StringBuilder();
            NameValueList pre = new NameValueList(pms);
            NameValueList _pms = new NameValueList();
            foreach (var fi in AllFields)
            {
                if (pre.ContainsKey(fi.Attribute.DbFieldName))
                {
                    if (iops == InsertType.New && fi.Key)
                        continue;
                    ss.AppendFormat("{0},", fi.EnsuredName);
                    vs.AppendFormat("@{0},", fi.Attribute.DbFieldName);
                    _pms.Add(GetFieldValue(fi, InsertType.None, pre[fi.Attribute.DbFieldName]));
                }
            }
            if (ss.Length > 0)
            {
                ss.Remove(ss.Length - 1, 1);
                vs.Remove(vs.Length - 1, 1);
            }
            return DbProvider.CreateCommand(string.Format("insert into {0} ({1}) values ({2})", EnsuredTableName, ss, vs), _pms);
        }
        public static int Insert(InsertType iops, Transaction trans, IEnumerable<NameValue> pms)
        {
            return GetInsertCommand(iops, pms).ExecuteNonQuery(trans);
        }
        public static Command GetInsertCommand(InsertType iops, params NameValue[] pms)
        {
            return GetInsertCommand(iops, pms as IEnumerable<NameValue>);
        }
        public static int Insert(InsertType iops, Transaction trans, params NameValue[] pms)
        {
            return GetInsertCommand(iops, pms as IEnumerable<NameValue>).ExecuteNonQuery(trans);
        }
        public static Object LastInsertKey(Transaction trans)
        {
            return DbProvider.CreateCommand(string.Format("select {0} from {1}", DbProvider.Adapter.LastInsertId(), EnsuredTableName)).ExecuteScalar(trans);
        }
        public virtual Command GetUpdateCommand(params string[] fields)
        {
            if (KeyField == null)
                return null;
            StringBuilder ss = new StringBuilder();
            foreach (var fi in AllFields)
            {
                if (fields.Length == 0 || fi.Attribute.DbFieldName.In(fields))
                    ss.AppendFormat("{0}=@{1},", fi.EnsuredName, fi.Attribute.DbFieldName);
            }
            if (ss.Length > 0)
                ss.Remove(ss.Length - 1, 1);
            string cmd = string.Format("update {0} set {1} where {2}=@{3}",
                EnsuredTableName, ss, KeyField.EnsuredName, KeyField.Attribute.DbFieldName);
            return DbProvider.CreateCommand(cmd, this.AsNameValueList(InsertType.None));
        }
        public int Update(Transaction trans, params string[] fields)
        {
            return GetUpdateCommand(fields).ExecuteNonQuery(trans);
        }
        public static Command GetUpdateCommand(Where where, IEnumerable<NameValue> pms)
        {
            if (pms.Count() == 0)
                return null;
            StringBuilder ss = new StringBuilder();
            NameValueList pre = new NameValueList(pms);
            NameValueList _pms = new NameValueList();
            foreach (var fi in AllFields)
            {
                if (pre.ContainsKey(fi.Attribute.DbFieldName))
                {
                    ss.AppendFormat("{0}=@{1},", fi.EnsuredName, fi.Attribute.DbFieldName);
                    _pms.Add(GetFieldValue(fi, InsertType.None, pre[fi.Attribute.DbFieldName]));
                }
            }
            if (ss.Length > 0)
                ss.Remove(ss.Length - 1, 1);
            Command cmd = DbProvider.CreateCommand(string.Format("update {0} set {1}", EnsuredTableName, ss), _pms);
            Builder.AppendWhereByEnsuredKey(cmd, where, KeyField.EnsuredName, DbProvider.Adapter);
            return cmd;
        }
        public static int Update(Where where, Transaction trans, IEnumerable<NameValue> pms)
        {
            return GetUpdateCommand(where, pms).ExecuteNonQuery(trans);
        }
        public static Command GetUpdateCommand(Where where, params NameValue[] pms)
        {
            return GetUpdateCommand(where, pms as IEnumerable<NameValue>);
        }
        public static int Update(Where where, Transaction trans, params NameValue[] pms)
        {
            return GetUpdateCommand(where, pms as IEnumerable<NameValue>).ExecuteNonQuery(trans);
        }
        public virtual Command GetReplaceCommand()
        {
            return DbProvider.Adapter.GetReplaceCommand(TableName, Key, KeyField.Attribute.DbFieldName, this.AsNameValueList(InsertType.None), DbProvider);
        }
        public int Replace(Transaction trans)
        {
            return GetReplaceCommand().ExecuteNonQuery(trans);
        }

        private void _swap_with(Where where, string fn, object cur, Transaction trans)
        {
            Transaction _trans;
            if (trans == null)
                _trans = new Transaction(DbProvider);
            else
                _trans = trans;
            var ano = GetSingle(where, new Select(KeyField.Attribute.DbFieldName, fn), trans) as DataEntityBase<T>;
            if (ano != null)
            {
                var t = ano.GetValue(fn);
                ano.SetValue(fn, cur);
                SetValue(fn, t);
                ano.Update(_trans, fn);
                Update(_trans, fn);
            }
            if (trans == null)
                _trans.Commit();
        }
        /// <summary>
        /// 若存在IndexField，则根据IndexField上移一位。
        /// </summary>
        public void MoveUp(Transaction trans)
        {
            if (IndexField != null)
            {
                var fn = IndexField.Attribute.DbFieldName;
                var cur = GetValue(fn);
                var where = new Compare(CompareType.GreaterThan, fn, cur).ToWhere();
                where.AddOrder(fn, OrderType.Asc);
                _swap_with(where, fn, cur, trans);
            }
            else
                throw new IndexFieldException("没有Index字段。", null);
        }
        /// <summary>
        /// 若存在IndexField，则根据IndexField下移一位。
        /// </summary>
        public void MoveDown(Transaction trans)
        {
            if (IndexField != null)
            {
                var fn = IndexField.Attribute.DbFieldName;
                var cur = GetValue(fn);
                var where = new Compare(CompareType.LessThan, fn, cur).ToWhere();
                where.AddOrder(fn, OrderType.Desc);
                _swap_with(where, fn, cur, trans);
            }
            else
                throw new IndexFieldException("没有Index字段。", null);
        }
        /// <summary>
        /// 若存在IndexField，则与指定目标交换位置。
        /// </summary>
        public void SwapWith(T obj, Transaction trans)
        {
            if (IndexField != null)
            {
                var fn = IndexField.Attribute.DbFieldName;
                var cur = GetValue(fn);
                var ano = obj as DataEntityBase<T>;
                SetValue(fn, ano.GetValue(fn));
                ano.SetValue(fn, cur);

                Transaction _trans;
                if (trans == null)
                    _trans = new Transaction(DbProvider);
                else
                    _trans = trans;
                Update(_trans, fn);
                ano.Update(_trans, fn);
                if (trans == null)
                    _trans.Commit();
            }
            else
                throw new IndexFieldException("没有Index字段。", null);
        }
        public void SetTop(bool IsTop, Transaction trans)
        {
            if (IndexField != null)
            {
                var fn = IndexField.Attribute.DbFieldName;
                var cur = Convert.ToInt64(GetValue(fn));
                if (IsTop && cur < TopLimit)
                {
                    cur += TopLimit;
                    SetValue(fn, Convert.ChangeType(cur, IndexField.Type));
                    Update(trans, fn);
                }
                else if (!IsTop && cur >= TopLimit)
                {
                    cur -= TopLimit;
                    SetValue(fn, Convert.ChangeType(cur, IndexField.Type));
                    Update(trans, fn);
                }
            }
            else
                throw new IndexFieldException("没有Index字段。", null);
        }

        #endregion

        public object Key
        {
            get
            {
                return GetValue(KeyField.Attribute.DbFieldName);
            }
            set
            {
                SetValue(KeyField.Attribute.DbFieldName, value);
            }
        }

        private static Cache<object, T> _cache = new Cache<object, T>();
        public static Cache<object, T> Cache
        {
            get { return _cache; }
        }

        #region DBInfo
        private static DbProvider _dbprovider;
        public static DbProvider DbProvider
        {
            get
            {
                if (_dbprovider == null)
                    _dbprovider = DbProvider.Current;
                return _dbprovider;
            }
            set { _dbprovider = value; }
        }

        private static DataEntityAttribute _dba;
        public static string TableName
        {
            get
            {
                if (_dba == null)
                    _dba = typeof(T).GetAttribute<DataEntityAttribute>(true);
                if(_dba == null)
                    return null;
                return _dba.TableName;
            }
        }

        public static bool UseCache
        {
            get
            {
                if (_dba == null)
                    _dba = typeof(T).GetAttribute<DataEntityAttribute>(true);
                if (_dba == null)
                    return false;
                return _dba.UseCache;
            }
        }

        public static Int64 TopLimit
        {
            get
            {
                if (_dba == null)
                    _dba = typeof(T).GetAttribute<DataEntityAttribute>(true);
                if (_dba == null)
                    return Int64.MaxValue;
                return _dba.TopLimit;
            }
        }

        public static Int64 StartIndex
        {
            get
            {
                if (_dba == null)
                    _dba = typeof(T).GetAttribute<DataEntityAttribute>(true);
                if (_dba == null)
                    return Int64.MinValue;
                return _dba.StartIndex;
            }
        }

        private static object _lock_initialize_dbinfo = new object();
        private static TableInfo _schema;
        public static void InitializeDbInfo()
        {
            lock (_lock_initialize_dbinfo)
            {
                if (_schema != null && _allFields != null)
                    return;
                _ensuredTableName = DbProvider.Adapter.EnsureIdentifier(TableName);
                _schema = DbProvider.Schema.GetTableInfo(TableName);
                if (_schema != null)
                {
                    //KeyField
                    foreach (var column_info in _schema.Columns.Values)
                        if (column_info.IsKey)
                        {
                            foreach (MemberInfo mi in typeof(T).GetFieldsAndProperties())
                            {
                                DataFieldAttribute a = mi.GetAttribute<DataFieldAttribute>();
                                if (a != null)
                                    if (a.DbFieldName == column_info.Name)
                                    {
                                        _keyField = new DataEntityFieldInfo();
                                        _keyField.MemberInfo = mi;
                                        _keyField.Attribute = a;
                                        _keyField.EnsuredName = DbProvider.Adapter.EnsureIdentifier(a.DbFieldName);
                                        _keyField.Type = mi.GetFieldOrPropertyType();
                                        _keyField.Name = a.DbFieldName;
                                        if (a.DisplayName == null)
                                            _keyField.DisplayName = a.DbFieldName;
                                        else
                                            _keyField.DisplayName = a.DisplayName;
                                        _keyField.Key = column_info.IsKey;
                                        _keyField.Nullable = column_info.IsNullable;
                                    }
                            }
                            break;
                        }
                    if (_keyField == null)
                        throw new DataEntityException("没有主键。", null);

                    //AllFields, IndexField
                    Dictionary<int, DataEntityFieldInfo> res = new Dictionary<int, DataEntityFieldInfo>();
                    foreach (MemberInfo mi in typeof(T).GetFieldsAndProperties())
                    {
                        DataFieldAttribute a = mi.GetAttribute<DataFieldAttribute>();
                        if (a != null)
                        {
                            DataEntityFieldInfo fi = new DataEntityFieldInfo();
                            fi.MemberInfo = mi;
                            fi.Attribute = a;
                            fi.EnsuredName = DbProvider.Adapter.EnsureIdentifier(a.DbFieldName);
                            fi.Type = mi.GetFieldOrPropertyType();
                            fi.Name = a.DbFieldName;
                            if (a.DisplayName == null)
                                fi.Name = a.DbFieldName;
                            else
                                fi.Name = a.DisplayName;
                            _schema.Columns[a.DbFieldName].Type = fi.Type;
                            _schema.Columns[a.DbFieldName].SerializeType = fi.Attribute.SerializeType;
                            fi.Key = _schema.Columns[a.DbFieldName].IsKey;
                            fi.Nullable = _schema.Columns[a.DbFieldName].IsNullable;
                            res.Add(_schema.Columns[a.DbFieldName].Index, fi);

                            if (mi.GetAttribute<IndexFieldAttribute>() != null && _indexField == null)
                                _indexField = fi;
                        }
                    }
                    var items = from k in res.Keys orderby k ascending select res[k];
                    _allFields = items.ToArray();
                }
            }
        }

        private static string _ensuredTableName;
        protected internal static string EnsuredTableName
        {
            get
            {
                if (_ensuredTableName == null)
                    InitializeDbInfo();
                return _ensuredTableName;
            }
        }

        private static DataEntityFieldInfo _keyField;
        public static DataEntityFieldInfo KeyField
        {
            get
            {
                if (_keyField == null)
                    InitializeDbInfo();
                return _keyField;
            }
        }

        private static DataEntityFieldInfo _indexField;
        public static DataEntityFieldInfo IndexField
        {
            get
            {
                if (_keyField == null)
                    InitializeDbInfo();
                return _indexField;
            }
        }

        private static DataEntityFieldInfo[] _allFields;
        public static DataEntityFieldInfo[] AllFields
        {
            get
            {
                if (_allFields == null)
                    InitializeDbInfo();
                return _allFields;
            }
        }

        private static MemberInfo[] _ownerFields;
        public static MemberInfo[] OwnerFields
        {
            get
            {
                if (_ownerFields == null)
                {
                    List<MemberInfo> res = new List<MemberInfo>();
                    foreach (MemberInfo mi in typeof(T).GetFieldsAndProperties())
                    {
                        DataOwnerFieldAttribute a = mi.GetAttribute<DataOwnerFieldAttribute>();
                        if (a != null)
                            res.Add(mi);
                    }
                    _ownerFields = res.ToArray();
                }
                return _ownerFields;
            }
        }
        #endregion

        public virtual object Clone()
        {
            return Clone(null);
        }
        public virtual T Clone(params string[] Fields)
        {
            if (_schema == null)
                InitializeDbInfo();
            T res = Activator.CreateInstance<T>();
            if (Fields == null || Fields.Length == 0)
            {
                foreach (var fi in AllFields)
                    (res as DataEntityBase<T>).SetValue(fi.Attribute.DbFieldName, GetValue(fi.Attribute.DbFieldName));
            }
            else
            {
                foreach (string field in Fields)
                {
                    if (_schema.Columns.ContainsKey(field))
                        (res as DataEntityBase<T>).SetValue(field, GetValue(field));
                }
            }
            return res;
        }
        public virtual string SerializeToJsonString()
        {
            StringBuilder res = new StringBuilder("{");
            foreach (var fi in AllFields)
                if (fi.MemberInfo.GetAttribute<JsonIgnoreAttribute>() == null)
                    res.Append(string.Format(@"""{0}"":{1},", fi.MemberInfo.Name, JsonUtils.Serialize(GetValue(fi.Attribute.DbFieldName))));
            foreach (var ofi in OwnerFields)
                if (ofi.GetAttribute<JsonIgnoreAttribute>() == null)
                    res.Append(string.Format(@"""{0}"":{1},", ofi.Name, JsonUtils.Serialize(ofi.GetValue(this))));
            if (res.Length > 1)
                res.Remove(res.Length - 1, 1);
            return res + "}";
        }
        public virtual void SetFieldValue(string fieldName, object value)
        {
            if (_schema == null)
                InitializeDbInfo();
            SetValue(fieldName, value);
        }
        public Type GetFieldType(string fieldName)
        {
            if (_schema == null)
                InitializeDbInfo();
            return _schema.Columns[fieldName].Type;
        }
        public object GetFieldValue(string fieldName)
        {
            if (_schema == null)
                InitializeDbInfo();
            return GetValue(fieldName);
        }

        #region IFieldViewable 成员

        IEnumerable<IFieldInfo> IFieldViewable.Fields
        {
            get { return AllFields; }
        }

        #endregion
    }
}
