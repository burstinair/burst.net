using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Burst.Data.Entity.CodeGenerate
{
    public class GFieldInfo
    {
        [Category("基本"), Description("字段在数据表中的名称。")]
        public string Name { get; set; }

        [Category("基本"), Description("生成的 Property 的名称。")]
        public string DisplayName { get; set; }

        internal string _db_type;
        [Category("基本"), Description("字段在数据库中的类型。")]
        public string DbType
        {
            get { return _db_type; }
            set { _db_type = value; }
        }

        [Category("基本"), Description("字段类型，类型限定名或在 \"gen:\" 后接 enum() 等。")]
        public string Type { get; set; }

        [Category("基本"), Description("字段在序列化方式。")]
        public SerializeType SerializeType { get; set; }

        [Category("基本"), Description("字段默认值。")]
        public object DefaultValue { get; set; }

        [Category("基本"), Description("字段是否为序号。")]
        public bool IsIndexField { get; set; }

        [Browsable(false)]
        public string FieldAttributes
        {
            get
            {
                var res = new List<string>();
                if (IsIndexField)
                    res.Add("Index");
                if (DefaultValue != null)
                    res.Add("Default(" + DefaultValue + ")");
                if (SerializeType != Burst.SerializeType.None)
                {
                    switch (SerializeType)
                    {
                        case Burst.SerializeType.Binary:
                            res.Add("SerializeType.Binary");
                            break;
                        case Burst.SerializeType.Xml:
                            res.Add("SerializeType.Xml");
                            break;
                        case Burst.SerializeType.Json:
                            res.Add("SerializeType.Json");
                            break;
                        case Burst.SerializeType.String:
                            res.Add("SerializeType.String");
                            break;
                    }
                }
                return string.Join(", ", res.ToArray());
            }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Type);
        }
    }
}
