using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Collections.Generic;
using Burst.Xml;
using Burst.Data.CommandBuilder;

namespace Burst.Data.Utils
{
    [DbConfiguration("configuration", SerializeType.Xml)]
    public class DbConfiguration<T>
    {
        private static T _singleton;
        private static T Singleton
        {
            get
            {
                if (_singleton == null)
                    _singleton = Activator.CreateInstance<T>();
                return _singleton;
            }
        }
        private static NameValueList _buf = null;
        protected static NameValueList OriData
        {
            get
            {
                if (_buf == null)
                    Reload(null);
                return _buf;
            }
        }

        protected void SetByAppSettings(NameValueList Data, string Key, Object DefaultValue)
        {
            if (Data == null)
                Data = OriData;
            Data[Key] = ConfigurationManager.AppSettings[Key];
            if (string.IsNullOrEmpty(Data.Get<string>(Key)))
                Data[Key] = DefaultValue;
        }
        protected virtual void Initialize()
        {
        }
        public static void Reload(Transaction trans, params string[] fields)
        {
            lock (Singleton)
            {
                if (_buf == null)
                    _buf = new NameValueList();

                Command cmd = new Command(string.Format("select * from {0}", EnsuredTableName));
                if (fields.Length > 0)
                    Builder.AppendWhere(
                        cmd,
                        new Compare(KeyFieldName, fields as object[]).ToWhere(),
                        KeyFieldName, DbProvider.Adapter
                    );
                else
                    _buf.Clear();

                NameValueList _old_buf = _buf;
                _buf = new NameValueList();
                (Singleton as DbConfiguration<T>).Initialize();
                foreach (var i in _buf)
                    if (!_old_buf.ContainsKey(i.Name))
                        _old_buf.Add(i);
                _buf = _old_buf;

                NameValueList[] res = cmd.Execute(trans);
                foreach (var nv in res)
                {
                    try
                    {
                        _buf.Add(
                            nv.Get<string>(KeyFieldName),
                            Burst.Utils.DeserializeAs(
                                nv.Get<object>(ValueFieldName),
                                Type.GetType(nv.Get<string>(TypeFieldName)),
                                SerializeType
                            )
                        );
                    }
                    catch { }
                }
            }
        }
        public static bool Save(Transaction trans, params string[] fields)
        {
            NameValueList config = OriData;
            lock (Singleton)
            {
                Transaction _trans = trans;
                if (_trans == null)
                    _trans = new Transaction(true);
                bool res = true;
                foreach (var nv in OriData)
                    if (fields.Length == 0 || nv.Name.In(fields))
                    {
                        object d = Burst.Utils.Serialize(nv.Value, SerializeType);
                        NameValueList pms = new NameValueList();
                        pms.Add("@" + KeyFieldName, nv.Name);
                        pms.Add("@" + TypeFieldName, nv.Value.GetType().AssemblyQualifiedName);
                        pms.Add("@" + ValueFieldName, d);
                        Command cmd = DbProvider.Adapter.GetReplaceCommand(TableName, nv.Name, KeyFieldName, pms, DbProvider);
                        res = cmd.ExecuteNonQuery(_trans) > -1 && res;
                    }
                if (trans != null)
                    return res;
                return _trans.Commit();
            }
        }
        public static bool Delete(Transaction trans, params string[] fields)
        {
            NameValueList config = OriData;
            lock (Singleton)
            {
                Transaction _trans = trans;
                if (_trans == null)
                    _trans = new Transaction(true);
                bool res = true;
                foreach (var nv in OriData)
                    if (fields.Length == 0 || nv.Name.In(fields))
                    {
                        Command cmd = new Command(string.Format("delete from {0}", EnsuredTableName));
                        Builder.AppendWhere(cmd, new Where(WhereType.Key, nv.Name), KeyFieldName, DbProvider.Adapter);
                        res = cmd.ExecuteNonQuery(_trans) > -1 && res;
                    }
                if (trans != null)
                    return res;
                return _trans.Commit();
            }
        }

        public static Tt Get<Tt>(string name)
        {
            NameValueList config = OriData;
            lock (Singleton)
            {
                if (config[name] == null)
                    return default(Tt);
                return Burst.Utils.DeserializeAs<Tt>(config[name]);
            }
        }
        public static void Set(string name, Object data)
        {
            NameValueList config = OriData;
            lock (Singleton)
            {
                config.Add(name, data);
            }
        }

        #region dbinfo

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

        private static string _ensuredTableName;
        protected internal static string EnsuredTableName
        {
            get
            {
                if (_ensuredTableName == null)
                    _ensuredTableName = DbProvider.Adapter.EnsureIdentifier(TableName);
                return _ensuredTableName;
            }
        }
        private static DbConfigurationAttribute _dca;
        public static string TableName
        {
            get
            {
                if (_dca == null)
                    _dca = TypeUtils.GetAttribute<DbConfigurationAttribute>(typeof(T), true);
                return _dca.TableName;
            }
        }
        public static SerializeType SerializeType
        {
            get
            {
                if (_dca == null)
                    _dca = TypeUtils.GetAttribute<DbConfigurationAttribute>(typeof(T), true);
                return _dca.SerializeType;
            }
        }
        public static string KeyFieldName
        {
            get
            {
                if (_dca == null)
                    _dca = TypeUtils.GetAttribute<DbConfigurationAttribute>(typeof(T), true);
                return _dca.KeyFieldName;
            }
        }
        public static string TypeFieldName
        {
            get
            {
                if (_dca == null)
                    _dca = TypeUtils.GetAttribute<DbConfigurationAttribute>(typeof(T), true);
                return _dca.TypeFieldName;
            }
        }
        public static string ValueFieldName
        {
            get
            {
                if (_dca == null)
                    _dca = TypeUtils.GetAttribute<DbConfigurationAttribute>(typeof(T), true);
                return _dca.ValueFieldName;
            }
        }

        #endregion
    }
}