using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;

namespace Burst.Windows
{
    public class WindowsUtils
    {
        /// <summary>
        /// 获取以当前路径为参考的相对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string MapPath(string path)
        {
            return System.Windows.Forms.Application.StartupPath + "\\" + path;
        }
        /// <summary>
        /// 将Image转换为BitmapImage
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static BitmapImage Convert(System.Drawing.Image source)
        {
            MemoryStream ms = new MemoryStream();
            source.Save(ms, ImageFormat.Png);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        /// <summary>
        /// 从字符串获取颜色
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static System.Windows.Media.Brush GetScBrush(string data)
        {
            System.Windows.Media.Color color;
            byte a, r, g, b;
            a = System.Convert.ToByte(data.Substring(0, 2), 16);
            r = System.Convert.ToByte(data.Substring(2, 4), 16);
            g = System.Convert.ToByte(data.Substring(4, 6), 16);
            b = System.Convert.ToByte(data.Substring(6, 8), 16);
            color = System.Windows.Media.Color.FromArgb(a, r, g, b);
            return new SolidColorBrush(color);
        }

        /// <summary>
        /// 获取资源文件中的图片
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ImageSource GetImage(string name)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Images/" + name));
        }

        /// <summary>
        /// 获取DependencyObject的T类型的子元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetChildObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            T grandChild = null;
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);
                if (child is T && (((T)child).Name == name || string.IsNullOrEmpty(name)))
                    return (T)child;
                else
                {
                    grandChild = GetChildObject<T>(child, name);
                    if (grandChild != null)
                        return grandChild;
                }
            }
            return null;
        }

        /// <summary>
        /// 控制按钮初始化
        /// </summary>
        /// <param name="b"></param>
        /// <param name="bkImage"></param>
        /// <param name="icoImage"></param>
        /// <param name="icoX"></param>
        public static void ControlButtonInit(System.Windows.Controls.Button b, string bkImage, string icoImage, double icoX)
        {
            System.Windows.Controls.Image light, ico;
            ImageSource img = GetImage(bkImage);
            GetChildObject<System.Windows.Controls.Image>(b, "bk").Source = img;
            light = GetChildObject<System.Windows.Controls.Image>(b, "light");
            light.Source = img;
            ico = GetChildObject<System.Windows.Controls.Image>(b, "ico");
            ico.Source = GetImage(icoImage);
            ico.Margin = new Thickness(icoX, 0, 0, 0);
        }
    }
}
