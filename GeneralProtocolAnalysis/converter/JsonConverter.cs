using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralProtocolAnalysis
{
    public class JsonConverter : IMessageConverter<string>
    {
        public JsonConverter(string protocol, string ver = null)
        {
            Protocol = AppSettings.GetProtocol(protocol, ver);
        }

        public JsonConverter(Protocol protocol)
        {
            Protocol = protocol;
        }

        public Protocol Protocol { get; private set; }

        public string Encode(Message message)
        {
            try
            {
                var value = new List<string>();
                foreach (var block in message.Blocks)
                {
                    value.Add(EncodeBlock(block));
                }

                return value.IsNullOrEmpty() ? null : ("{" + string.Join(", ", value) + "}");
            }
            catch (Exception ex)
            {
                throw new Exception($"the message can not encode to binary stream, {ex.Message}");
            }
        }

        public Message Decode(string target)
        {
            throw new NotImplementedException();
        }

        public Message Decode(string target, string messageName)
        {
            if (target.IsNotNullOrEmpty() && messageName.IsNotNullOrEmpty())
            {
                var message = Protocol.Messages.FirstOrDefault(x => x.Name == messageName);
                if (message != null && message.Blocks.IsNotNullOrEmpty() && message.Size > 0)
                {
                    message = message.Copy();
                    foreach (var block in message.Blocks)
                    {
                        DecodeBlock(block, JToken.Parse(target));
                    }
                }

                return message;
            }

            return null;
        }

        private string EncodeBlock(Block block)
        {
            var value = string.Empty;
            if (block != null)
            {
                switch (block.Type)
                {
                    case TypeCode.DateTime:
                        value = string.Format("\"{0}\":{1}", block.Name, ((DateTime)block.Value).ToString(block.Format));
                        break;
                    case TypeCode.Char:
                    case TypeCode.String:
                        value = string.Format("\"{0}\":\"{1}\"", block.Name, block.Value);
                        break;
                    case TypeCode.Object:
                        if (block.Blocks.IsNotNullOrEmpty())
                        {
                            value = "\"" + block.Name + "\":{";
                            //var current = "[";
                            for (int i = 0; i < block.Blocks.Count; i++)
                            {
                                value += EncodeBlock(block.Blocks[i]);
                                if (i != block.Blocks.Count - 1)
                                {
                                    value += ",";
                                }
                            }

                            value += "}";
                        }
                        break;
                    case TypeCode.Boolean:
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                    default:
                        value = string.Format("\"{0}\":{1}", block.Name, block.Value);
                        break;
                }
            }

            return value;
        }

        private void DecodeBlock(Block block, JToken token)
        {
            if (token != null && token.HasValues)
            {
                switch (block.Type)
                {
                    case TypeCode.DateTime:
                        block.Value = token[block.Name].Value<DateTime>();
                        break;
                    case TypeCode.Object:
                        if (block.Blocks.IsNotNullOrEmpty())
                        {
                            var blockToken = token[block.Name];
                            foreach (var bl in block.Blocks)
                            {
                                DecodeBlock(bl, blockToken);
                            }
                        }
                        break;
                    case TypeCode.Char:
                    case TypeCode.String:
                        block.Value = token[block.Name].Value<string>();
                        break;
                    case TypeCode.Boolean:
                        block.Value = token[block.Name].Value<bool>();
                        break;
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                        block.Value = Convert.ChangeType(token[block.Name].Value<long>(), block.Type);
                        break;
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        block.Value = Convert.ChangeType(token[block.Name].Value<decimal>(), block.Type);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
