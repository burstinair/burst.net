using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Burst
{
    public class LocalConfigurationAttribute : Attribute
    {
        protected string _fileName;
        public string FileName { get { return _fileName; } }

        public LocalConfigurationAttribute(string FileName)
        {
            this._fileName = FileName;
        }
    }
}
