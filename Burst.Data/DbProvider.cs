using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Data.Common;

using Burst.Data.Schema;

namespace Burst.Data
{
    public delegate bool DBReaderDelegate(DbDataReader r);

    public class DbProvider
    {
        private NameValueList _config;
        private DbProvider()
        {
            this._config = new NameValueList();
        }

        public static DbProvider Current
        {
            get;
            set;
        }

        private static T _getInstance<T>(Object obj)
        {
            if (obj is T)
                return (T)obj;
            else if(obj is string)
                return (T)Activator.CreateInstance(Type.GetType(obj as string));
            return default(T);
        }

        /// <summary>
        /// 从默认配置中初始化数据连接参数，包括连接字符串(ConnectionString)、DbProviderFactory、DbAdapter。
        /// </summary>
        /// <returns>初始化成功时，返回DbProvider，否则，返回null。</returns>
        public static DbProvider Initialize()
        {
            DbConnection _conn = null;
            DbProvider res = new DbProvider();
            try
            {
                res._config["DbAdapter"] = Activator.CreateInstance(Type.GetType(ConfigurationManager.AppSettings["DbAdapter"]));
                res.Adapter.InitializeOptions(res.Options);
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["DbProviderFactory"]) && res.Options["DefaultDbProviderFactory"] != null)
                    res._config["DbProviderFactory"] = _getInstance<DbProviderFactory>(res.Options["DefaultDbProviderFactory"]);
                else
                    res._config["DbProviderFactory"] = Activator.CreateInstance(Type.GetType(ConfigurationManager.AppSettings["DbProviderFactory"]));
                res._config["ConnectionString"] = ConfigurationManager.AppSettings["ConnectionString"];
                if (string.IsNullOrEmpty(res.ConnectionString))
                {
                    NameValueList DbParameters = new NameValueList();
                    DbParameters.Add("Host", ConfigurationManager.AppSettings["DbHost"]);
                    DbParameters.Add("Database", ConfigurationManager.AppSettings["DbDatabase"]);
                    DbParameters.Add("FilePath", ConfigurationManager.AppSettings["DbFilePath"]);
                    DbParameters.Add("Username", ConfigurationManager.AppSettings["DbUsername"]);
                    DbParameters.Add("Password", ConfigurationManager.AppSettings["DbPassword"]);
                    DbParameters.Add("Charset", ConfigurationManager.AppSettings["DbCharset"]);
                    DbParameters.Add("Version", ConfigurationManager.AppSettings["DbVersion"]);
                    DbParameters.Add("ReadOnly", ConfigurationManager.AppSettings["DbReadOnly"]);
                    res._config["ConnectionString"] = res.Adapter.BuildConnectionString(DbParameters);
                }
                _conn = res.Provider.CreateConnection();
                _conn.ConnectionString = res.ConnectionString;
                _conn.Open();
                res._config["DbSchema"] = res.Adapter.GetSchema(_conn);
                if (Current == null)
                    Current = res;
                return res;
            }
            catch (Exception e)
            {
                #if DEBUG
                DbConnectException dce = new DbConnectException(e, res._config);
                Debug.WriteLine(dce);
                throw dce;
                #endif

                return null;
            }
            finally
            {
                try
                {
                    _conn.Close();
                }
                catch { }
            }
        }

        /// <summary>
        /// 使用连接字符串、DbProviderFactory、DbAdapter初始化数据连接参数。
        /// </summary>
        /// <param name="ConnectionString">连接字符串</param>
        /// <param name="DbProviderFactory">DbProviderFactory</param>
        /// <param name="DbAdapter">DbAdapter</param>
        /// <returns>初始化成功时，返回DbProvider，否则，返回null。</returns>
        public static DbProvider Initialize(string ConnectionString, DbProviderFactory DbProviderFactory, IDbAdapter DbAdapter)
        {
            DbConnection _conn = null;
            DbProvider res = new DbProvider();
            try
            {
                if (DbAdapter != null)
                {
                    res._config["DbAdapter"] = DbAdapter;
                    DbAdapter.InitializeOptions(res.Options);
                }
                if (DbProviderFactory != null)
                    res._config["DbProviderFactory"] = DbProviderFactory;
                else if (res.Options["DefaultDbProviderFactory"] != null)
                    res._config["DbProviderFactory"] = _getInstance<DbProviderFactory>(res.Options["DefaultDbProviderFactory"]);
                if (!string.IsNullOrEmpty(ConnectionString))
                    res._config["ConnectionString"] = ConnectionString;
                _conn = res.Provider.CreateConnection();
                _conn.ConnectionString = res.ConnectionString;
                _conn.Open();
                res._config["DbSchema"] = res.Adapter.GetSchema(_conn);
                if (Current == null)
                    Current = res;
                return res;
            }
            catch (Exception e)
            {
                #if DEBUG
                DbConnectException dce = new DbConnectException(e, res._config);
                Debug.WriteLine(dce);
                throw dce;
                #endif

                return null;
            }
            finally
            {
                try
                {
                    _conn.Close();
                }
                catch { }
            }
        }

        /// <summary>
        /// 使用若干参数初始化数据连接参数。
        /// </summary>
        /// <param name="Parameters">参数，包括DbProviderFactory(String类型表示的完整限定名称或DbProviderFactory类型)、DbAdapter(String类型表示的类型完整限定名称或DbAdapter类型)，及连接字符串(ConnectionString)或当前IDbAdapter.BuildConnectionString所需参数。</param>
        /// <returns>初始化成功时，返回DbProvider，否则，返回null。</returns>
        public static DbProvider Initialize(NameValueList Parameters)
        {
            DbConnection _conn = null;
            DbProvider res = new DbProvider();
            try
            {
                IDbAdapter DbAdapter = _getInstance<IDbAdapter>(Parameters["DbAdapter"]);
                if (DbAdapter != null)
                {
                    res._config["DbAdapter"] = DbAdapter;
                    DbAdapter.InitializeOptions(res.Options);
                }
                DbProviderFactory DbProviderFactory = _getInstance<DbProviderFactory>(Parameters["DbProviderFactory"]);
                if (DbProviderFactory != null)
                    res._config["DbProviderFactory"] = DbProviderFactory;
                else if (res.Options["DefaultDbProviderFactory"] != null)
                    res._config["DbProviderFactory"] = _getInstance<DbProviderFactory>(res.Options["DefaultDbProviderFactory"]);
                if (Parameters["ConnectionString"] is string)
                    res._config["ConnectionString"] = Parameters["ConnectionString"];
                else
                    res._config["ConnectionString"] = res.Adapter.BuildConnectionString(Parameters);
                _conn = res.Provider.CreateConnection();
                _conn.ConnectionString = res.ConnectionString;
                _conn.Open();
                res._config["DbSchema"] = res.Adapter.GetSchema(_conn);
                if (Current == null)
                    Current = res;
                return res;
            }
            catch (Exception e)
            {
                #if DEBUG
                DbConnectException dce = new DbConnectException(e, res._config);
                Debug.WriteLine(dce);
                throw dce;
                #endif

                return null;
            }
            finally
            {
                try
                {
                    _conn.Close();
                }
                catch { }
            }
        }

        /// <summary>
        /// 当前所使用的DbProviderFactory。
        /// </summary>
        public DbProviderFactory Provider
        {
            get
            {
                if (_config == null)
                    Initialize();
                return _config.Get<DbProviderFactory>("DbProviderFactory");
            }
        }
        /// <summary>
        /// 当前所使用的连接字符串。
        /// </summary>
        public string ConnectionString
        {
            get
            {
                if (_config == null)
                    Initialize();
                return _config.Get<string>("ConnectionString");
            }
        }
        private object _open_connection_lock = new object();
        private bool _locking = false;
        public DbConnection CreateConnection()
        {
            if (_locking)
            {
                lock (_open_connection_lock)
                {
                    DbConnection res = Provider.CreateConnection();
                    res.ConnectionString = ConnectionString;
                    return res;
                }
            }
            else
            {
                DbConnection res = Provider.CreateConnection();
                res.ConnectionString = ConnectionString;
                return res;
            }
        }
        public void LockAndRun(Action dele)
        {
            lock (_open_connection_lock)
            {
                _locking = true;
                if (dele != null)
                    dele();
                _locking = false;
            }
        }

        public Command CreateCommand(string cmd, params object[] pms)
        {
            Command res = new Command(cmd, pms);
            res.DbProvider = this;
            return res;
        }
        public Command CreateCommand(string cmd, NameValueList pms)
        {
            Command res = new Command(cmd, pms);
            res.DbProvider = this;
            return res;
        }

        /// <summary>
        /// 当前所使用的IDbAdapter。
        /// </summary>
        public IDbAdapter Adapter
        {
            get
            {
                if (_config == null)
                    Initialize();
                return _config.Get<IDbAdapter>("DbAdapter");
            }
        }
        /// <summary>
        /// 当前数据库的架构
        /// </summary>
        public DbSchema Schema
        {
            get
            {
                if (_config == null)
                    Initialize();
                return _config.Get<DbSchema>("DbSchema");
            }
        }
        public NameValueList Options
        {
            get
            {
                if (_config == null)
                    Initialize();
                if (_config.Get<NameValueList>("Options") == null)
                    _config["Options"] = new NameValueList();
                return _config.Get<NameValueList>("Options");
            }
        }

        public static int ConditionExecuteNonQuery(Command JudgeCommand, IEnumerable<Command> DefaultCommands, IEnumerable<Command> SubCommands, Transaction trans)
        {
            Debug.Assert(JudgeCommand != null, "DbProvider.ConditionExecuteNonQuery: JudgeCommand Cannot be NULL");
            if (JudgeCommand == null)
                return -1;
            try
            {
                if (JudgeCommand.ExecuteScalar(trans) != null && DefaultCommands != null)
                    return Extension.ExecuteNonQuery(DefaultCommands, trans);
                else if (SubCommands != null)
                    return Extension.ExecuteNonQuery(SubCommands, trans);
                else
                    return -1;
            }
            catch (Exception e)
            {
                #if DEBUG
                    CommandException ce = new CommandException("ConditionExecuteNonQuery", JudgeCommand, e);
                    Debug.WriteLine(ce);
                    throw ce;
                #endif
            }
            return -1;
        }
    }
}
