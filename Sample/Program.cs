using System;
using GeneralProtocolAnalysis;

namespace Sample
{
    class Program
    {
        private const string protocolName = "ipcell";
        private const string messageName = "MSG_START_REQ";

        static void Main(string[] args)
        {
            try
            {
                var hexString = "00 38 03 E9 30 30 30 31 00 00 00 01 00 00 00 00 00 00 00 00 01 01 00 00 00 00 00 00 01 30 30 30 00 00 00 00 01 30 30 30 00 01 00 00 00 00 00 00 00 01 00 08 01 01 00 00";
                var bytes = hexString.ToBinary();
                var value = bytes.ToHexString();
                Console.WriteLine(value);
                Console.WriteLine(value == hexString);
                Console.WriteLine(protocolName);
                //JsonBinaryConverterTest(bytes);
                //Console.WriteLine(JsonConverterTest() == hexString);
                JsonHexStringConverterTest(hexString);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.ReadLine();
            }
        }

        static void JsonBinaryConverterTest(byte[] bytes)
        {
            var converter = new JsonBinaryConvert(protocolName);
            var json = converter.Decode(bytes, messageName);
            Console.WriteLine(json);
            var result = converter.Encode(json, messageName);
            Console.WriteLine(result.ToHexString());
        }

        static string JsonConverterTest()
        {
            var value = string.Empty;
            var converter = new JsonBinaryConvert(protocolName);
            var json = AppSettings.GetConfig($"/config/message/{messageName}.json");
            if (json.IsNotNullOrEmpty())
            {
                var bytes = converter.Encode(json, messageName);
                if (bytes.IsNotNullOrEmpty())
                {
                    value = bytes.ToHexString(" ");
                    Console.WriteLine(value);
                }
            }

            return value;
        }

        static void JsonHexStringConverterTest(string hexString)
        {
            var converter = new JsonHexStringConverter(protocolName);
            var json = converter.Decode(hexString, messageName);
            Console.WriteLine(json);
            var result = converter.Encode(json, messageName);
            Console.WriteLine(result);
            Console.WriteLine(result == hexString);
        }
    }
}
