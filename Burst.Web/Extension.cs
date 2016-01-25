using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace Burst.Web
{
    public static class Extension
    {
        public static void Set(this Page page, string key, object value)
        {
            page.Session[string.Format("{0}_{1}", page.GetType().FullName, key)] = value;
        }

        public static T Get<T>(this Page page, string key)
        {
            return (T)page.Session[string.Format("{0}_{1}", page.GetType().FullName, key)];
        }
    }
}
