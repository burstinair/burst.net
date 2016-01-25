using System;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web;
using System.Web.UI;
using System.Web.Caching;
using System.Collections.Generic;
using System.Text;

namespace Burst.Web.CMS
{
    public abstract class Section<T> : WebControl, INamingContainer
    {
        #region Cache
        private static Dictionary<string, string> contents = new Dictionary<string, string>();
        protected string Contents
        {
            get
            {
                if (!contents.ContainsKey(Key))
                {
                    lock (contents)
                    {
                        if (!contents.ContainsKey(Key))
                            contents.Add(Key, UpdateContents());
                        return contents[Key];
                    }
                }
                return contents[Key];
            }
        }
        public static void ClearContents(params string[] keys)
        {
            lock (contents)
            {
                if (keys.Length == 0)
                    contents.Clear();
                else
                    foreach (var k in keys)
                        contents.Remove(k);
            }
        }
        public abstract string UpdateContents();
        #endregion

        #region override
        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }
        protected override void RenderContents(HtmlTextWriter writer)
        {
            if(ShowTitle)
                writer.Write(string.Format("<div class='title'>{0}</div>", Title));
            writer.Write(Contents);
        }
        #endregion

        #region property

        [Bindable(true), Category("Section"), DefaultValue("")]
        [PersistenceMode(PersistenceMode.Attribute)]
        public virtual string Key { get; set; }

        [Bindable(true), Category("Section"), DefaultValue("")]
        [PersistenceMode(PersistenceMode.Attribute)]
        public virtual string Title { get; set; }

        [Bindable(true), Category("Section"), DefaultValue(true)]
        [PersistenceMode(PersistenceMode.Attribute)]
        public virtual bool ShowTitle { get; set; }

        #endregion

        public Section()
        {
            this.ShowTitle = true;
            this.Title = string.Empty;
            this.Key = string.Empty;
        }
    }
}
