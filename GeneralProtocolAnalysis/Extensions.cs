using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneralProtocolAnalysis
{
    public static class Extensions
    {
        public static T Copy<T>(this T t)
        {
            try
            {
                var setting = new JsonSerializerSettings();
                setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                setting.Converters.Add(new StringEnumConverter());
                var json = JsonConvert.SerializeObject(t, setting);
                return JsonConvert.DeserializeObject<T>(json, setting);
            }
            catch (Exception ex)
            {
                throw new Exception($"type of {typeof(T).Name} can not be copy! {ex.Message}");
            }
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNotNullOrEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source != null && source.Any();
        }

        public static string ToHexString(this IEnumerable<byte> bytes, string spliter = " ")
        {
            if (bytes.IsNotNullOrEmpty())
            {
                var value = BitConverter.ToString(bytes.ToArray());
                if (spliter != "-")
                {
                    value = value.Replace("-", spliter);
                }

                return value;
            }

            return null;
        }

        public static byte[] ToBinary(this string hexString, string spliter = " ")
        {
            if (!string.IsNullOrEmpty(hexString))
            {
                if (string.IsNullOrEmpty(spliter))
                {
                    var bytes = new byte[hexString.Length / 2];
                    for (int i = 0; i < hexString.Length; i += 2)
                    {
                        bytes[i] = Convert.ToByte(hexString.Substring(i, 2), 16);
                    }

                    return bytes;
                }
                else
                {
                    var binaryArray = hexString.Split(spliter);
                    if (binaryArray.Length > 0)
                    {
                        var bytes = new byte[binaryArray.Length];
                        for (int i = 0; i < binaryArray.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(binaryArray[i]))
                            {
                                bytes[i] = Convert.ToByte(binaryArray[i], 16);
                            }
                        }

                        return bytes;
                    }
                }
            }

            return null;
        }
    }
}
