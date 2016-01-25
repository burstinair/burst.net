using System;
using System.Collections.Generic;
using System.Text;
using Burst.Json;

namespace Burst
{
    [Serializable]
    public class SearchResult
    {
        public object[] Data { get; set; }
        public int StartPos { get; set; }
        public int Count { get; set; }
        public int Total { get; set; }
    }

    [Serializable]
    public class SearchResult<T> : SearchResult
    {
        public new T[] Data
        {
            get
            {
                return base.Data as T[];
            }
            set
            {
                base.Data = value as object[];
            }
        }
    }
}
