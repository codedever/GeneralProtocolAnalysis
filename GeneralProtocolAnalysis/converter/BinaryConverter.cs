using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralProtocolAnalysis
{
    public class BinaryConverter : IMessageConverter<byte[]>
    {
        public BinaryConverter(string protocolName, string version = null)
        {
            Protocol = AppSettings.GetProtocol(protocolName, version);
        }

        public BinaryConverter(Protocol protocol)
        {
            Protocol = protocol;
        }

        public Protocol Protocol { get; private set; }

        public byte[] Encode(Message source)
        {
            try
            {
                var bytes = new List<byte>();
                if (source != null && source.Protocol == Protocol && source.Size > 0)
                {
                    foreach (var block in source.Blocks)
                    {
                        bytes.AddRange(EncodeBlock(block));
                    }
                }

                return bytes.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception($"the message can not encode to binary stream, {ex.Message}");
            }
        }

        public Message Decode(byte[] target)
        {
            throw new NotImplementedException();
        }

        public Message Decode(byte[] target, string messageName)
        {
            if (target.IsNotNullOrEmpty() && messageName.IsNotNullOrEmpty())
            {
                var message = Protocol.Messages.FirstOrDefault(x => x.Name == messageName);
                if (message != null && message.Blocks.IsNotNullOrEmpty() && message.Size > 0)
                {
                    message = message.Copy();
                    var startIndex = 0;
                    foreach (var block in message.Blocks)
                    {
                        DecodeBlock(block, target, ref startIndex);
                    }
                }

                return message;
            }

            return null;
        }

        private byte[] EncodeBlock(Block block)
        {
            byte[] bytes = null;
            try
            {
                if (block != null && block.Size > 0 && (block.Type == TypeCode.Object || block.Value != null))
                {
                    switch (block.Type)
                    {
                        case TypeCode.Boolean:
                            if (block.Value is bool boolValue)
                            {
                                bytes = BitConverter.GetBytes(boolValue);
                            }
                            break;
                        case TypeCode.Byte:
                            if (block.Value is byte byteValue)
                            {
                                bytes = new byte[] { byteValue };
                            }
                            break;
                        case TypeCode.Char:
                            if (block.Value is char charValue)
                            {
                                bytes = BitConverter.GetBytes(charValue);
                            }
                            break;
                        case TypeCode.DateTime:
                            if (block.Value is DateTime dateTime)
                            {
                                bytes = BitConverter.GetBytes(dateTime.Ticks);
                            }
                            break;
                        case TypeCode.Decimal:
                        case TypeCode.Double:
                            if (block.Value is double doubleValue)
                            {
                                bytes = BitConverter.GetBytes(doubleValue);
                            }
                            break;
                        case TypeCode.Int16:
                            if (block.Value is short shortValue)
                            {
                                bytes = BitConverter.GetBytes(shortValue);
                            }
                            break;
                        case TypeCode.Int32:
                            if (block.Value is int intValue)
                            {
                                bytes = BitConverter.GetBytes(intValue);
                            }
                            break;
                        case TypeCode.Int64:
                            if (block.Value is long longValue)
                            {
                                bytes = BitConverter.GetBytes(longValue);
                            }
                            break;
                        case TypeCode.SByte:
                            if (block.Value is sbyte sbyteValue)
                            {
                                //todo sbyte 转 byte 可能有经度丢失
                                bytes = new byte[] { Convert.ToByte(sbyteValue) };
                            }
                            break;
                        case TypeCode.Single:
                            if (block.Value is float floatValue)
                            {
                                bytes = BitConverter.GetBytes(floatValue);
                            }
                            break;
                        case TypeCode.UInt16:
                            if (block.Value is ushort ushortValue)
                            {
                                bytes = BitConverter.GetBytes(ushortValue);
                            }
                            break;
                        case TypeCode.UInt32:
                            if (block.Value is uint uintValue)
                            {
                                bytes = BitConverter.GetBytes(uintValue);
                            }
                            break;
                        case TypeCode.UInt64:
                            if (block.Value is ulong ulongValue)
                            {
                                bytes = BitConverter.GetBytes(ulongValue);
                            }
                            break;
                        case TypeCode.String:
                            if (block.Encoding.IsNotNullOrEmpty())
                            {
                                var current = Encoding.GetEncoding(block.Encoding).GetBytes(block.Value as string);
                                if (current.Length < block.Size)
                                {
                                    bytes = new byte[block.Size];
                                    for (int i = 0; i < block.Size - bytes.Length; i++)
                                    {
                                        bytes[i] = AppSettings.PADDING_CHAR;
                                    }

                                    Array.Copy(current, 0, bytes, current.Length - 1, current.Length);
                                }
                                else
                                {
                                    bytes = current;
                                }
                            }
                            break;
                        case TypeCode.Object:
                            if (block.Blocks.IsNotNullOrEmpty())
                            {
                                var list = new List<byte>();
                                foreach (var x in block.Blocks)
                                {
                                    list.AddRange(EncodeBlock(x));
                                }

                                bytes = list.ToArray();
                            }
                            break;
                        case TypeCode.DBNull:
                        case TypeCode.Empty:
                        default:
                            if (block.Size != 0)
                            {
                                bytes = new byte[block.Size];
                                for (int i = 0; i < block.Size; i++)
                                {
                                    bytes[i] = AppSettings.PADDING_NUMBER;
                                }
                            }
                            break;
                    }

                    if (BitConverter.IsLittleEndian && block.Type >= TypeCode.Int16 && block.Type <= TypeCode.Decimal)
                    {
                        Array.Reverse(bytes);
                    }
                }

                if (bytes.IsNullOrEmpty())
                {
                    throw new Exception($"the block can not be encode to binary stream!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"the block can not be encode to binary stream! {ex.Message}");
            }

            return bytes;
        }

        private void DecodeBlock(Block block, byte[] bytes, ref int startIndex)
        {
            if (block != null && block.Size > 0 && bytes.IsNotNullOrEmpty())
            {
                if (BitConverter.IsLittleEndian && block.Type >= TypeCode.Int16 && block.Type <= TypeCode.Decimal)
                {
                    Array.Reverse(bytes, startIndex, block.Size);
                }

                switch (block.Type)
                {
                    case TypeCode.Boolean:
                        block.Value = BitConverter.ToBoolean(bytes, startIndex);
                        break;
                    case TypeCode.Byte:
                        block.Value = Convert.ToByte(bytes[startIndex]);
                        break;
                    case TypeCode.Char:
                        block.Value = BitConverter.ToChar(bytes, startIndex);
                        break;
                    case TypeCode.DateTime:
                        block.Value = new DateTime(BitConverter.ToInt64(bytes, startIndex));
                        break;
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                        block.Value = BitConverter.ToDouble(bytes, startIndex);
                        break;
                    case TypeCode.Int16:
                        block.Value = BitConverter.ToInt16(bytes, startIndex);
                        break;
                    case TypeCode.Int32:
                        block.Value = BitConverter.ToInt32(bytes, startIndex);
                        break;
                    case TypeCode.Int64:
                        block.Value = BitConverter.ToInt32(bytes, startIndex);
                        break;
                    case TypeCode.SByte:
                        block.Value = Convert.ToSByte(bytes[startIndex]);
                        break;
                    case TypeCode.Single:
                        block.Value = BitConverter.ToSingle(bytes, startIndex);
                        break;
                    case TypeCode.UInt16:
                        block.Value = BitConverter.ToUInt16(bytes, startIndex);
                        break;
                    case TypeCode.UInt32:
                        block.Value = BitConverter.ToUInt32(bytes, startIndex);
                        break;
                    case TypeCode.UInt64:
                        block.Value = BitConverter.ToUInt64(bytes, startIndex);
                        break;
                    case TypeCode.String:
                        if (block.Encoding.IsNotNullOrEmpty())
                        {
                            block.Value = Encoding.GetEncoding(block.Encoding).GetString(bytes, startIndex, block.Size);
                        }
                        break;
                    case TypeCode.Object:
                        if (block.Blocks.IsNotNullOrEmpty())
                        {
                            foreach (var x in block.Blocks)
                            {
                                DecodeBlock(x, bytes, ref startIndex);
                            }
                        }
                        break;
                    case TypeCode.DBNull:
                    case TypeCode.Empty:
                    default:
                        block.Value = null;
                        break;
                }

                if (block.Type != TypeCode.Object)
                {
                    startIndex += block.Size;
                }
            }
        }
    }
}
