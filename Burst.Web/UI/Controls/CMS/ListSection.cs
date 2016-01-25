using System;
using System.ComponentModel;
using System.Web.UI;
using System.Collections.Generic;
using System.Text;

namespace Burst.Web.CMS
{
    public abstract class ListSection<T> : Section<T>
    {
        [Bindable(true), Category("Section"), DefaultValue(10)]
        [PersistenceMode(PersistenceMode.Attribute)]
        public virtual int ShowCount { get; set; }

        [Bindable(true), Category("Section"), DefaultValue("")]
        [PersistenceMode(PersistenceMode.Attribute)]
        public virtual string HeaderImg { get; set; }

        [Bindable(true), Category("Section"), DefaultValue("")]
        [PersistenceMode(PersistenceMode.Attribute)]
        public virtual string HeaderImgKey { get; set; }

        public ListSection()
        {
            this.ShowCount = 10;
            this.HeaderImg = string.Empty;
            this.HeaderImgKey = string.Empty;
        }
    }
}
