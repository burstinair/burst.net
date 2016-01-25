using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Burst.Data;

namespace Burst.Data.CommandBuilder
{
    public class Page
    {
        public int StartPos { get; set; }
        public int Count { get; set; }

        public Page(int startPos, int count)
        {
            this.StartPos = startPos;
            this.Count = count;
        }
    }
}
