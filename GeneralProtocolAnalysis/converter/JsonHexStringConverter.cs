using System;
using System.Collections.Generic;
using System.Text;

namespace GeneralProtocolAnalysis
{
    public class JsonHexStringConverter : IConverter<string, string>
    {
        public JsonHexStringConverter(string protocol, string ver = null, string spliter = " ")
        {
            Protocol = AppSettings.GetProtocol(protocol, ver);
            BinaryConverter = new BinaryConverter(protocol, ver);
            JsonConverter = new JsonConverter(protocol, ver);
            Spliter = spliter;
        }

        public Protocol Protocol { get; private set; }
        private string Spliter { get; set; }
        private IConverter<Block, byte[]> BinaryConverter { get; set; }
        private IConverter<Block, string> JsonConverter { get; set; }

        public string Decode(string target, string message = null)
        {
            return JsonConverter.Encode(BinaryConverter.Decode(target?.ToBinary(Spliter), message));
        }

        public string Encode(string source, string message = null)
        {
            return BinaryConverter.Encode(JsonConverter.Decode(source, message))?.ToHexString(Spliter);
        }
    }
}
