namespace GeneralProtocolAnalysis
{
    public class JsonHexStringConverter
    {
        public JsonHexStringConverter(string protocol, string ver = null, string spliter = " ")
        {
            Protocol = AppSettings.GetProtocol(protocol, ver);
            Spliter = spliter;
            BinaryConverter = new BinaryConverter(protocol, ver);
            JsonConverter = new JsonConverter(protocol, ver);
        }

        public JsonHexStringConverter(Protocol protocol, string spliter = " ")
        {
            Protocol = protocol;
            Spliter = spliter;
            BinaryConverter = new BinaryConverter(Protocol);
            JsonConverter = new JsonConverter(protocol);
        }

        public Protocol Protocol { get; private set; }
        private string Spliter { get; set; }
        private IMessageConverter<byte[]> BinaryConverter { get; set; }
        private IMessageConverter<string> JsonConverter { get; set; }

        public string Decode(string target, string message)
        {
            return JsonConverter.Encode(BinaryConverter.Decode(target?.ToBinary(Spliter), message));
        }

        public string Encode(string source, string message)
        {
            return BinaryConverter.Encode(JsonConverter.Decode(source, message))?.ToHexString(Spliter);
        }
    }
}
