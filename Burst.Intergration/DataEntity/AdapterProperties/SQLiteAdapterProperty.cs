using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Burst;
using Burst.Data;
using Burst.Data.Entity.CodeGenerate;
using Burst.Data.SQLite;

using System.IO;

namespace BurstStudio.Burst_Intergration.DataEntity.AdapterProperties
{
    public enum SQLiteVersion
    {
        V2, V3
    }

    public class SQLiteAdapterProperty : AdapterProperty
    {
        public override string ToString()
        {
            return "SQLite Adapter";
        }

        public SQLiteAdapterProperty()
        {
            pms["DbAdapter"] = new SQLiteAdapter();
        }

        [Category("指定 ConnectionString"), Description("指定 ConnectionString, 指定后将忽视生成 ConnectionString 的设置。")]
        public override string ConnectionString
        {
            get { return pms.Get<string>("ConnectionString"); }
            set { pms["ConnectionString"] = value; }
        }

        [System.ComponentModel.Editor(
            typeof(System.Windows.Forms.Design.FileNameEditor),
            typeof(System.Drawing.Design.UITypeEditor))]
        [Category("生成 ConnectionString"), Description("要连接到的文件。")]
        public string FilePath
        {
            get { return pms.Get<string>("FilePath") ?? AdapterProperty.SelectedPath; }
            set { pms["FilePath"] = value; }
        }

        [Category("生成 ConnectionString"), Description("版本")]
        public SQLiteVersion Version
        {
            get { return pms.Get<string>("Version") == "2" ? SQLiteVersion.V2 : SQLiteVersion.V3; }
            set { pms["Version"] = value == SQLiteVersion.V2 ? "2" : "3"; }
        }

        [Category("生成 ConnectionString"), Description("密码。")]
        public string Password
        {
            get { return pms.Get<string>("Password"); }
            set { pms["Password"] = value; }
        }
    }
}
