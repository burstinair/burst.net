using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Burst;
using Burst.Data;
using Burst.Data.Entity.CodeGenerate;
using Burst.Data.MSAccess;

using System.IO;

namespace BurstStudio.Burst_Intergration.DataEntity.AdapterProperties
{
    public class MSAccessAdapterProperty : AdapterProperty
    {
        public override string ToString()
        {
            return "Microsoft Access Adapter";
        }

        public MSAccessAdapterProperty()
        {
            pms["DbAdapter"] = new MSAccessAdapter();
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

        [Category("生成 ConnectionString"), Description("密码。")]
        public string Password
        {
            get { return pms.Get<string>("Password"); }
            set { pms["Password"] = value; }
        }
    }
}
