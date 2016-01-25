using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Burst
{
    public abstract class AbstractPic
    {
        public string id;
        public int width, height;

        public abstract void ToStream(Stream s);
        public abstract Image ToImage();
    }
}
