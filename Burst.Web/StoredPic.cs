using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Xml.Serialization;

namespace Burst.Web
{
    public class StoredPic : AbstractPic, ICustomSerializeObject
    {
        protected string Path
        {
            get
            {
                return WebUtils.PhysicalPath(id);
            }
        }

        public override void ToStream(Stream s)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(Path, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                byte[] buffer = br.ReadBytes((int)fs.Length);
                s.Write(buffer, 0, buffer.Length);
            }
            catch { }
            finally
            {
                try
                {
                    fs.Close();
                }
                catch { }
            }
        }
        public override Image ToImage()
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(Path, FileMode.Open, FileAccess.Read);
                Image img = Image.FromStream(fs);
                width = img.Width;
                height = img.Height;
                return img;
            }
            catch { }
            finally
            {
                try
                {
                    fs.Close();
                }
                catch { }
            }
            return null;
        }
        public bool SaveBy(Image img)
        {
            if (string.IsNullOrEmpty(id))
                return false;
            width = img.Width;
            height = img.Height;
            FileStream fs = null;
            try
            {
                fs = new FileStream(Path, FileMode.Create, FileAccess.Write);
                img.Save(fs, ImageFormat.Png);
                return true;
            }
            catch { }
            finally
            {
                try
                {
                    fs.Close();
                }
                catch { }
            }
            return false;
        }

        public StoredPic()
        {
        }
        public StoredPic(string id)
        {
            this.id = id;
        }

        void ICustomSerializeObject.Deserialize(Object obj)
        {
            if (obj is string)
                this.id = Utils.DeserializeAs<string>(obj);
        }
        public override string ToString()
        {
            return id;
        }
        Object ICustomSerializeObject.Serialize()
        {
            return ToString();
        }
    }
}
