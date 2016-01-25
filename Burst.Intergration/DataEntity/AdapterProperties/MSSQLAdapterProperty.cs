using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Burst;
using Burst.Data;
using Burst.Data.Entity.CodeGenerate;
using Burst.Data.MSSQL;

namespace BurstStudio.Burst_Intergration.DataEntity.AdapterProperties
{
    public class MSSQLAdapterProperty : AdapterProperty
    {
        public override string ToString()
        {
            return "Microsoft SQLServer Adapter";
        }

        public MSSQLAdapterProperty()
        {
            pms["DbAdapter"] = new MSSQLAdapter();
        }


        [Category("指定 ConnectionString"), Description("指定 ConnectionString, 指定后将忽视生成 ConnectionString 的设置。")]
        public override string ConnectionString
        {
            get { return pms.Get<string>("ConnectionString"); }
            set { pms["ConnectionString"] = value; }
        }

        [Category("生成 ConnectionString"), Description("要连接到的主机。")]
        public string Host
        {
            get { return pms.Get<string>("Host"); }
            set { pms["Host"] = value; }
        }

        [Category("生成 ConnectionString"), Description("要连接到的数据库。")]
        public string Database
        {
            get { return pms.Get<string>("Database"); }
            set { pms["Database"] = value; }
        }

        [Category("生成 ConnectionString"), Description("用户名。")]
        public string Username
        {
            get { return pms.Get<string>("Username"); }
            set { pms["Username"] = value; }
        }

        [Category("生成 ConnectionString"), Description("密码。")]
        public string Password
        {
            get { return pms.Get<string>("Password"); }
            set { pms["Password"] = value; }
        }
    }
}
