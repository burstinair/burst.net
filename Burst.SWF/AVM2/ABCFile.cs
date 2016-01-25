using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Burst.SWF.AVM2
{
    public class ABCFile : Structure
    {
        public override void ReadFrom(ABCReader reader)
        {
            MinorVersion = reader.ReadUInt16();
            MajorVersion = reader.ReadUInt16();

            ConstantPool.ReadFrom(reader);
            ConstantPool.Initialize();

            uint MethodCount = (uint)(reader.ReadEncodedInt());
            Method = new Method[MethodCount];
            for (int i = 0; i < MethodCount; i++)
            {
                Method[i] = new Method();
                Method[i].ReadFrom(reader);
            }

            uint MetadataCount = (uint)(reader.ReadEncodedInt());
            Metadata = new MetaData[MetadataCount];
            for (int i = 0; i < MetadataCount; i++)
            {
                Metadata[i] = new MetaData();
                Metadata[i].ReadFrom(reader);
            }

            uint ClassCount = (uint)(reader.ReadEncodedInt());
            Instance = new Instance[ClassCount];
            for (int i = 0; i < ClassCount; i++)
            {
                Instance[i] = new Instance();
                Instance[i].ReadFrom(reader);
            }
            Class = new Class[ClassCount];
            for (int i = 0; i < ClassCount; i++)
            {
                Class[i] = new Class();
                Class[i].ReadFrom(reader);
            }

            uint ScriptCount = (uint)(reader.ReadEncodedInt());
            Script = new Script[ScriptCount];
            for (int i = 0; i < ScriptCount; i++)
            {
                Script[i] = new Script();
                Script[i].ReadFrom(reader);
            }

            uint MethodBodyCount = (uint)(reader.ReadEncodedInt());
            MethodBody = new MethodBody[MethodBodyCount];
            for (int i = 0; i < MethodBodyCount; i++)
            {
                MethodBody[i] = new MethodBody();
                MethodBody[i].ReadFrom(reader);
            }
        }

        public UInt16 MinorVersion
        {
            get { return (UInt16)this["minor_version"]; }
            set { this["minor_version"] = value; }
        }
        public UInt16 MajorVersion
        {
            get { return (UInt16)this["major_version"]; }
            set { this["major_version"] = value; }
        }
        public ConstantPool ConstantPool
        {
            get { return (ConstantPool)this["constant_pool"]; }
            set { this["constant_pool"] = value; }
        }
        public Method[] Method
        {
            get { return (Method[])this["method"]; }
            set { this["method"] = value; }
        }
        public MetaData[] Metadata
        {
            get { return (MetaData[])this["metadata"]; }
            set { this["metadata"] = value; }
        }
        public Instance[] Instance
        {
            get { return (Instance[])this["instance"]; }
            set { this["instance"] = value; }
        }
        public Class[] Class
        {
            get { return (Class[])this["class"]; }
            set { this["class"] = value; }
        }
        public Script[] Script
        {
            get { return (Script[])this["script"]; }
            set { this["script"] = value; }
        }
        public MethodBody[] MethodBody
        {
            get { return (MethodBody[])this["method_body"]; }
            set { this["method_body"] = value; }
        }
    }
}
