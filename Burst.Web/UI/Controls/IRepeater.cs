using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Burst.Web.UI.Controls
{
    public interface IRepeater
    {
        void SetItems(IEnumerable Items);
    }
}
