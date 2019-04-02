using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneralProtocolAnalysis
{
    public class Protocol
    {
        internal bool Initialized { get; private set; }
        public string Name { get; set; }
        public string Doc { get; set; }
        public string Ver { get; set; }
        public string Encoding { get; set; }
        public ProtocolType Type { get; set; }
        public Block Header { get; set; }
        public List<Message> Messages { get; set; }

        public void Init()
        {
            if (!Initialized)
            {
                if (Header != null && Header.Blocks.IsNotNullOrEmpty())
                {
                    foreach (var block in Header.Blocks)
                    {
                        block.Init();
                    }

                    Header.Size = Header.Blocks.Sum(x => x.Size);

                    foreach (var message in Messages)
                    {
                        message.Protocol = this;
                        message.Init();
                    }
                }

                Initialized = true;
            }
        }
    }

    public class Message
    {
        internal bool Initialized { get; private set; }
        public Protocol Protocol { get; internal set; }
        public int Size { get; set; }
        public string Name { get; set; }
        public string Doc { get; set; }
        public List<Block> Header { get; set; }
        public List<Block> Body { get; set; }

        internal void Init()
        {
            if (!Initialized)
            {
                if (Header == null && Protocol.Header != null && Protocol.Header.Blocks.IsNotNullOrEmpty())
                {
                    Header = Protocol.Header.Blocks.Copy();
                }

                if (Header != null)
                {
                    foreach (var block in Header)
                    {
                        block.Init();
                    }

                    Size = Header.Sum(x => x.Size);
                }

                if (Body.IsNotNullOrEmpty())
                {
                    foreach (var block in Body)
                    {
                        block.Init();
                    }

                    Size += Body.Sum(x => x.Size);
                }

                Initialized = true;
            }
        }
    }

    public class Block
    {
        public Block()
        {
            Size = 1;
        }

        internal bool Initialized { get; private set; }
        internal Block Parent { get; private set; }
        public int Size { get; set; }
        public string Name { get; set; }
        public string Doc { get; set; }
        public TypeCode Type { get; set; }
        public object Value { get; set; }
        public string Encoding { get; set; }
        public string Format { get; set; }
        public List<Block> Blocks { get; set; }

        /// <summary>
        /// Blocks 初始化之后, 包含 Protocol 的 Header 部分
        /// </summary>
        internal void Init()
        {
            if (!Initialized)
            {
                if (Blocks.IsNotNullOrEmpty())
                {
                    Size = 0;
                    if (Type == TypeCode.Empty)
                    {
                        Type = TypeCode.Object;
                    }

                    foreach (var block in Blocks)
                    {
                        block.Parent = this;
                        block.Init();
                        Size += block.Size;
                    }
                }

                if (Type == TypeCode.Empty)
                {
                    if (Size == 1)
                    {
                        Type = TypeCode.SByte;
                    }
                    else if (Size == 2)
                    {
                        Type = TypeCode.Int16;
                    }
                    else if (Size == 4)
                    {
                        Type = TypeCode.Int32;
                    }
                    else if (Size == 8)
                    {
                        Type = TypeCode.Int64;
                    }
                    else
                    {
                        Type = TypeCode.String;
                    }
                }

                Encoding = Encoding ?? AppSettings.AppSetting.DefaultEncoding;
                if (Type == TypeCode.DateTime && Format.IsNullOrEmpty())
                {
                    Format = AppSettings.AppSetting.DateTimeFormat;
                }

                Initialized = true;
            }
        }
    }
}
