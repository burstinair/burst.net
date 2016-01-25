using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Burst.Web
{
    interface IServiceInvoker
    {
        void Initialize(string location);
        void BeginFunction(string key, int pm, AsyncCallback callback, AsyncState state);
        string EndFunction(IAsyncResult r);
    }
}
