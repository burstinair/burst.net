using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Burst.Data.Entity.CodeGenerate
{
    public class OwnerFieldInfo
    {
        [Category("基本"), Description("字段在数据表中的名称。")]
        public string Name { get; set; }

        [Category("基本"), Description("生成的 Property 的名称。")]
        public string DisplayName { get; set; }

        [Category("基本"), Description("该 Owner 字段包含实体的类型。")]
        public string OwnedEntityType { get; set; }

        [Category("基本"), Description("字段类型，在 \"ori:\" 后接类型限定名或在 \"gen:\" 后接 enum() 等。")]
        public string Type { get; set; }

        [Category("基本"), Description("加载对象时是否自动加载该 Owner 字段。")]
        public bool AutoLoad { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1}: {2})", Name, Type, OwnedEntityType);
        }
    }
}
