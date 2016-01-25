using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace Burst.Data
{
    public class DbConnectException : Exception
    {
        public string ConnectionString;
        public DbProviderFactory DbProviderFactory;
        public IDbAdapter DbAdapter;

        public override string Message
        {
            get
            {
                return string.Format("无法连接到数据库。\nConnectionString: {0}\nDbProviderFactory: {1}\nDbAdapter: {2}\n{3}", ConnectionString, DbProviderFactory, DbAdapter, InnerException);
            }
        }

        public override string StackTrace
        {
            get
            {
                return InnerException.StackTrace;
            }
        }

        public DbConnectException(Exception InnerException)
            : base(string.Empty, InnerException)
        {
        }
        public DbConnectException(Exception InnerException, NameValueList Config)
            : base(string.Empty, InnerException)
        {
            this.ConnectionString = Config.Get<string>("ConnectionString");
            this.DbProviderFactory = Config.Get<DbProviderFactory>("DbProviderFactory");
            this.DbAdapter = Config.Get<IDbAdapter>("DbAdapter");
        }
    }
}
