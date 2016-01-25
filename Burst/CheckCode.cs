using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;

namespace Burst
{
    public class CheckCode : AbstractPic
    {
        public bool outputed = false;
        public string code;
                
        public static CheckCode Generate()
        {
            CheckCode cc = new CheckCode();
            string list = "abcdefghijklmnopqrstuvwxyz";
            cc.code = "";
            Random r = new Random();
            for (int i = 0; i < 5; i++)
                cc.code += list[r.Next(26)];
            return cc;
        }

        public override void ToStream(Stream s)
        {
            Image image = ToImage();
            try
            {
                MemoryStream ms = new MemoryStream();
                image.Save(ms, ImageFormat.Jpeg);
                ms.WriteTo(s);
            }
            finally
            {
                image.Dispose();
            }
        }
        public override Image ToImage()
        {
            Bitmap image = new Bitmap(130, 53);
            Graphics pic = Graphics.FromImage(image);
            try
            {
                Random random = new Random();
                Brush fc = new SolidBrush(new Color[] {
                        Color.FromArgb(255, 44, 89, 127),
                        Color.FromArgb(255, 81, 126, 19),
                        Color.FromArgb(255, 183, 68, 5)
                    }[random.Next(3)]);
                string ff = new string[] {
                        "Arial", "Cambria", "Georgia", "Calibri"
                    }[random.Next(4)];
                Font font = new Font(ff, 20, FontStyle.Bold);
                pic.SmoothingMode = SmoothingMode.AntiAlias;
                pic.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                pic.Clear(Color.FromArgb(255, 248, 248, 248));
                int last = random.Next(5) + 5;
                for (int i = 0; i < 5; i++)
                {
                    int ro = random.Next(90) - 45;
                    float sc = random.Next(5) / 5f + 0.9f;
                    pic.ResetTransform();
                    pic.TranslateTransform(last + 13, 20);
                    pic.RotateTransform(ro);
                    pic.ScaleTransform(sc, sc);
                    pic.TranslateTransform(-last - 13, -20);
                    pic.Flush();
                    string des = code[i].ToString().ToLower();
                    if (random.Next(2) == 0)
                        des = des.ToUpper();
                    pic.DrawString(des, font, fc, last, random.Next(15) + 2);
                    last += 25 - last / 10;
                }
                pic.ResetTransform();
                for (int j = 0; j < 3; j++)
                {
                    List<Point> list = new List<Point>();
                    for (int i = 0; i < 4; i++)
                        list.Add(new Point(i * 43, random.Next(50)));
                    pic.DrawBeziers(new Pen(fc, random.Next(3)), list.ToArray());
                }
                return image;
            }
            finally
            {
                pic.Dispose();
            }
        }
    }
}
