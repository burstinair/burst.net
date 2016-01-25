using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;

using Burst;
using Burst.Data;
using Burst.Data.Entity.CodeGenerate;

namespace BurstStudio.Burst_Intergration.DataEntity.AdapterProperties
{
    public class NameValuePair
    {
        private string _name;
        public string Name
        {
            get
            {
                if (_name == null)
                    return "";
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        private string _value;
        public string Value
        {
            get
            {
                if (_value == null)
                    return "";
                return _value;
            }
            set
            {
                _value = value;
            }
        }
    }

    public class AdapterProperty
    {
        protected NameValueList pms;
        internal NameValueList Parameters
        {
            get
            {
                NameValueList res = new NameValueList(pms);
                foreach (NameValuePair nvp in _extra)
                    res.Add(nvp.Name, nvp.Value);
                return res;
            }
        }

        public virtual string ConnectionString { get; set; }
        internal static string SelectedPath;

        public AdapterProperty()
        {
            pms = new NameValueList();
        }

        private List<NameValuePair> _extra = new List<NameValuePair>();

        [Category("生成 ConnectionString"), Description("详细配置。")]
        public List<NameValuePair> DetailConfiguration
        {
            get
            {
                return _extra;
            }
            set
            {
                _extra = value;
            }
        }
    }
}
