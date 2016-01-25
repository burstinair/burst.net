using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Burst.SWF.Zip;

namespace Burst.SWF
{
    public static class Helper
    {
        private static HelperBase _current = new ChineseHelper();
        public static HelperBase Current
        {
            get { return _current; }
            set { _current = value; }
        }

        private static ZipHelperBase _zip_helper = new IonicZlibHelper();
        public static ZipHelperBase ZipHelper
        {
            get { return _zip_helper; }
            set { _zip_helper = value; }
        }
        
        public static string GetString(SWFFileType Type)
        {
            return Current.GetString(Type);
        }

        public static string GetString(SWFTagType Type)
        {
            return Current.GetString(Type);
        }

        public static string GetString(SWFFile File)
        {
            return Current.GetString(File);
        }

        public static string GetInfo(SWFTag Tag)
        {
            return Current.GetInfo(Tag);
        }

        public static string GetString(SWFTag Tag)
        {
            return Current.GetString(Tag);
        }

        public static string GetHexString(byte[] bytes, int index, int endindex)
        {
            return Current.GetHexString(bytes, index, endindex);
        }
    }
}
