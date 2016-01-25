using System;
using System.Collections.Generic;
using System.Text;

namespace Burst.Web.UI
{
    public class Page<T> : AuthenticationPage<T>
    {
        protected override void OnInit(EventArgs e)
        {
            this.Init += (object sender, EventArgs _e) =>
            {
                var tip = this.FindControl("Tip") as Burst.Web.UI.Controls.Tip;
                if (tip == null && this.Master != null)
                    tip = this.Master.FindControl("Tip") as Burst.Web.UI.Controls.Tip;
                this.Tip = tip;
            };
            this.Init += new EventHandler(Page_Init);
            this.Load += new EventHandler(Page_Load);
            this.Load += (object sender, EventArgs _e) =>
            {
                DealEvent();
            };
            base.OnInit(e);
        }

        protected virtual Dictionary<string, object> GetParameters()
        {
            var res = new Dictionary<string, object>();
            if (!RequestIsAjax)
            {
                //TODO
            }
            return res;
        }

        protected virtual void DealEvent()
        {
            BaseException ex = SuccessException.Singleton;

            try
            {
                var _event = EventName;
                if (!String.IsNullOrEmpty(_event))
                {
                    if (handlers.ContainsKey(_event))
                        handlers[_event](GetParameters());
                    else
                        ex = new BaseException();
                }
                else
                    ex = null;
            }
            catch (BaseException bex)
            {
                ex = bex;
            }
            catch (Exception e)
            {
                ex = new BaseException(e);
            }

            if (RequestIsAjax)
            {
                Response.Write(ex.ClientTip);
                Response.End();
            }
            else
            {
                if (Tip != null)
                    Tip.Show(ex);
            }
        }

        private Dictionary<string, PageEventHandler> handlers = new Dictionary<string, PageEventHandler>();

        protected void AddEventHandler(string EventName, PageEventHandler handler)
        {
            if (!handlers.ContainsKey(EventName))
                handlers[EventName] = handler;
            else
                handlers[EventName] = PageEventHandler.Combine(handlers[EventName], handler) as PageEventHandler;
        }

        protected virtual Burst.Web.UI.Controls.Tip Tip { get; set; }

        protected virtual string EventName
        {
            get { return Request["_EVENTNAME"]; }
        }
        protected virtual bool RequestIsAjax
        {
            get { return Request["_ISAJAX"] == "1"; }
        }

        protected virtual void Page_Load(object sender, EventArgs e) { }

        protected virtual void Page_Init(object sender, EventArgs e) { }
    }
}
