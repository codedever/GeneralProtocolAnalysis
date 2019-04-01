namespace GeneralProtocolAnalysis
{
    public class JsonBinaryConvert
    {
        public JsonBinaryConvert(string protocol, string ver = null)
        {
            Protocol = AppSettings.GetProtocol(protocol, ver);
            BinaryConverter = new BinaryConverter(protocol, ver);
            JsonConverter = new JsonConverter(protocol, ver);
        }

        public JsonBinaryConvert(Protocol protocol)
        {
            Protocol = protocol;
            BinaryConverter = new BinaryConverter(Protocol);
            JsonConverter = new JsonConverter(protocol);
        }

        public Protocol Protocol { get; private set; }
        private IMessageConverter<byte[]> BinaryConverter { get; set; }
        private IMessageConverter<string> JsonConverter { get; set; }

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
