using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Burst.SWF
{
    public class SWFFile
    {
        public string Type { get; set; }
        public double FrameRate { get; set; }
        public int Version { get; set; }
        public int Size { get; set; }
        public int FrameCount { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public List<SWFTag> Tags;

        protected BinaryReader reader;

        protected int getExp(int a, int b)
        {
            int c = 1;
            for (int i = 0; i < b; i++)
                c *= a;
            return c;
        }
        public SWFFile(Stream s)
        {
            this.reader = new BinaryReader(s);
            var ntype = new string(reader.ReadChars(3));
            if (!(ntype == "CWS") && !(ntype == "FWS"))
                throw new SWFAnalysisException("SWF File Format Error at 0.", null);

            Version = reader.ReadByte();
            Size = reader.ReadInt32();

            if (ntype == "CWS")
                reader = new BinaryReader(Helper.ZipHelper.GetDefluateStream(
                    reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position)))
                );
            Type = ntype;

            int[] stagesize = GetRect(reader);
            Width = stagesize[1] / 20;
            Height = stagesize[3] / 20;

            FrameRate = reader.ReadInt16();
            FrameCount = reader.ReadInt16();

            Tags = new List<SWFTag>();
            while (true)
            {
                int t = reader.ReadInt16();
                if (t == 0)
                    break;
                int id = t >> 6;
                int length = t & 63;
                if (length == 63)
                    length = reader.ReadInt32();
                byte[] bbb;
                if (length > 0)
                    bbb = reader.ReadBytes(length);
                else
                    bbb = new byte[0];
                Tags.Add(new SWFTag(id, bbb));
            }

            reader.Close();
            s.Close();
        }

        private void Swap(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length / 2; i++)
                bytes[i] = bytes[bytes.Length - i - 1];
        }

        private int[] GetRect(BinaryReader br)
        {
            byte bt = br.ReadByte();
            int t = Int32.Parse(Math.Round(((bt >> 3) * 4 + 5) / 8.0 + 0.5).ToString()) - 1;
            byte[] buf = br.ReadBytes(t);
            byte[] data = new byte[buf.Length + 1];
            data[0] = bt;
            buf.CopyTo(data, 1);
            int[] res = new int[4];
            BitArray ba = new BitArray(data);
            int len = ba.GetInt(0, 5);
            res[0] = ba.GetInt(5, len);
            res[1] = ba.GetInt(5 + len, len);
            res[2] = ba.GetInt(5 + len * 2, len);
            res[3] = ba.GetInt(5 + len * 3, len);
            return res;
        }

        public override string ToString()
        {
            return Helper.GetString(this);
        }
    }
}
