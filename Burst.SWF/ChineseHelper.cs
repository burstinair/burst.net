using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Burst.SWF
{
    public class ChineseHelper : HelperBase
    {
        public override string GetString(SWFFileType Type)
        {
            switch (Type)
            {
                default:
                case SWFFileType.Unknown:
                    return "Unknown";
                case SWFFileType.CWS:
                    return "CWS";
                case SWFFileType.FWS:
                    return "FWS";
            }
        }
        public override string GetString(SWFTagType Type)
        {
            switch (Type)
            {
                case SWFTagType.Unknown:
                    return "未知";
                case SWFTagType.ShowFrame:
                    return "显示帧";
                case SWFTagType.DefineShape:
                case SWFTagType.DefineShape2:
                case SWFTagType.DefineShape3:
                    return "图形";
                case SWFTagType.SetBackGroundColor:
                    return "背景色";
                case SWFTagType.DefineText:
                    return "文本";
                case SWFTagType.DefineSound:
                    return "声音";
                case SWFTagType.DefineBitsJPEG2:
                case SWFTagType.DefineBitsJPEG3:
                case SWFTagType.DefineBitsJPEG4:
                    return "JPEG图片";
                case SWFTagType.PlaceObject:
                case SWFTagType.PlaceObject2:
                    return "添加物体";
                case SWFTagType.RemoveObject:
                case SWFTagType.RemoveObject2:
                    return "删除物体";
                case SWFTagType.FrameLabel:
                    return "帧标签";
                case SWFTagType.DefineFont:
                case SWFTagType.DefineFont2:
                case SWFTagType.DefineFont3:
                case SWFTagType.DefineFont4:
                    return "字体";
                case SWFTagType.ExportAssets:
                    return "导出资源";
                case SWFTagType.FileAttributes:
                    return "文件属性";
                default:
                    return Type.ToString();
            }
        }
        public override string GetString(SWFFile File)
        {
            StringBuilder a = new StringBuilder();
            a.AppendFormat("类型：{0}\r\n", File.Type);
            a.AppendFormat("版本：{0}\r\n", File.Version);
            a.AppendFormat("大小：{0}字节\r\n", File.Size);
            a.AppendFormat("尺寸(W*H)：{0}*{1}\r\n", File.Width, File.Height);
            a.AppendFormat("帧频：{0}", File.FrameRate / 256.0);
            a.AppendFormat(" 帧数：{0}\r\n", File.FrameCount);
            return a.ToString();
        }
        public override string GetString(SWFTag Tag)
        {
            return string.Format("[{0}]({1}){2}", Tag.Content.Length, Tag.Id, GetString(Tag.Type));
        }

        public override string GetInfo(SWFTag Tag)
        {
            StringBuilder res = new StringBuilder();
            MemoryStream ms;
            BinaryReader br;
            switch (Tag.Type)
            {
                case SWFTagType.ShowFrame:
                    break;
                case SWFTagType.DefineText:
                    ms = new MemoryStream(Tag.Content);
                    br = new BinaryReader(ms);
                    res.AppendFormat("ID:{0}\n", br.ReadInt16());
                    res.Append(GetHexString(Tag.Content, 2, Tag.Content.Length));
                    break;
                case SWFTagType.FrameLabel:
                    res.Append(Encoding.UTF8.GetString(Tag.Content));
                    break;
                case SWFTagType.ExportAssets:
                    ms = new MemoryStream(Tag.Content);
                    br = new BinaryReader(ms);
                    res.AppendFormat("数量:{0}\n", br.ReadInt16());
                    res.AppendFormat("第一个资源的ID:{0}\n", br.ReadInt16());
                    res.AppendFormat("名字:{0}", new String(br.ReadChars(Tag.Content.Length - 4)));
                    break;
                case SWFTagType.DefineSprite:
                    ms = new MemoryStream(Tag.Content);
                    br = new BinaryReader(ms);
                    res.AppendFormat("ID:{0}\n", br.ReadInt16());
                    res.AppendFormat("帧数:{0}\n", br.ReadInt16());
                    res.Append(GetHexString(Tag.Content, 4, Tag.Content.Length));
                    break;
                case SWFTagType.DoABC:
                    //TODO
                    res.Append(GetHexString(Tag.Content, 0, Tag.Content.Length));
                    break;
                default:
                    res.Append(GetHexString(Tag.Content, 0, Tag.Content.Length));
                    break;
            }
            return res.ToString();
        }
    }
}
