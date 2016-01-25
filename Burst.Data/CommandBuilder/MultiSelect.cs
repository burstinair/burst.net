using System;
using System.Collections.Generic;
using System.Text;

namespace Burst.Data.CommandBuilder
{
    public class MultiSelect : Select
    {
        public List<string> Tables { get; set; }
    }
}
