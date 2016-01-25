using System;
using System.Web;
using System.Collections.Generic;
using System.Text;

namespace Burst.Web.UI
{
    public class BasePage<T> : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            if (!IsPostBack)
                try
                {
                    Session.Remove(_type_name);
                }
                catch { }
            base.OnInit(e);
        }

        private static string _type_name = "Page_Context_" + typeof(T).AssemblyQualifiedName;

        private Dictionary<object, object> _buffer
        {
            get
            {
                var res = Session[_type_name] as Dictionary<object, object>;
                if (res == null)
                {
                    res = new Dictionary<object, object>();
                    Session[_type_name] = res;
                }
                return res;
            }
        }

        public TF Get<TF>(string key)
        {
            try
            {
                return (TF)_buffer[key];
            }
            catch
            {
                return default(TF);
            }
        }

        public void Set(string key, object value)
        {
            _buffer[key] = value;
        }
    }
}
