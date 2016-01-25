using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Burst.Data.Entity.CodeGenerate;

namespace BurstStudio.Burst_Intergration.DataEntity.AdapterProperties
{
    public class GeneralProperty
    {
        [Category("基本"), Description("编辑要生成的数据实体类的名称。")]
        public string ClassName { get; set; }

        [Category("基本"), Description("编辑要生成的数据实体类对应的数据表的名称。")]
        public string TableName { get; set; }

        [Category("基本"), Description("是否从 Burst.Web.UserBase 派生。")]
        public bool IsWebUserModel { get; set; }

        [Category("基本"), Description("是否使用缓存。")]
        public bool UseCache { get; set; }

        [Category("基本"), Description("编辑要生成的数据实体的字段。")]
        public List<GFieldInfo> Fields { get; set; }

        [Category("Owner字段"), Description("编辑要生成的数据实体的 Owner 字段。")]
        public List<OwnerFieldInfo> OwnerFields { get; set; }

        public override string ToString()
        {
            return TableName;
        }
    }
}
