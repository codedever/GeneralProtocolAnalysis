using System;
using System.Collections.Generic;
using System.Text;

namespace GeneralProtocolAnalysis
{
    public class JsonBinaryConvert : IConverter<string, byte[]>
    {
        public JsonBinaryConvert(string protocol, string ver = null)
        {
            Protocol = AppSettings.GetProtocol(protocol, ver);
            BinaryConverter = new BinaryConverter(protocol, ver);
            JsonConverter = new JsonConverter(protocol, ver);
        }

        public Protocol Protocol { get; private set; }
        private IConverter<Block, byte[]> BinaryConverter { get; set; }
        private IConverter<Block, string> JsonConverter { get; set; }

        /// <summary>
        /// 将 json 字符串编码为字节流
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public byte[] Encode(string source, string message)
        {
            return BinaryConverter.Encode(JsonConverter.Decode(source, message));
        }

        /// <summary>
        /// 将字节流解码为 json 字符串
        /// </summary>
        /// <param name="message"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public string Decode(byte[] target, string message)
        {
            return JsonConverter.Encode(BinaryConverter.Decode(target, message));
        }
    }
}
