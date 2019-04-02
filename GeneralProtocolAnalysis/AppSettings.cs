using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GeneralProtocolAnalysis
{
    /// <summary>
    /// App Setting in app.json
    /// </summary>
    public static class AppSettings
    {
        /// <summary>
        /// �ļ���չ���ָ���
        /// </summary>
        public const char FILE_EXTENSION_SEPARATOR = '.';
        /// <summary>
        /// �ַ����λ
        /// </summary>
        public const byte PADDING_CHAR = 0x20;
        /// <summary>
        /// �������λ
        /// </summary>
        public const byte PADDING_NUMBER = 0x00;
        /// <summary>
        /// �����ļ���
        /// </summary>
        private const string CONFIG_FILE_NAME = "/config/app.json";
        /// <summary>
        /// application startup path
        /// </summary>
        public static readonly string StartupPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static AppSetting AppSetting { get; private set; }
        /// <summary>
        /// �Ѷ�ȡ��ϵͳ������
        /// </summary>
        public static IEnumerable<Protocol> Protocols { get; private set; }

        static AppSettings()
        {
            var json = GetConfig(CONFIG_FILE_NAME);
            AppSetting = GetSettings<AppSetting>(json);
            if (AppSetting != null && AppSetting.ProtocolPath.IsNotNullOrEmpty())
            {
                GetProtocols();
            }
        }

        /// <summary>
        /// ͨ���ļ�����ȡ�����ļ�����
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetConfig(string fileName)
        {
            var fullName = fileName;
            if (!File.Exists(fullName))
            {
                fullName = StartupPath + fileName;
            }

            if (File.Exists(fullName))
            {
                var streamReader = new StreamReader(fullName);
                try
                {
                    return streamReader.ReadToEnd();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    streamReader.Close();
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// json�ַ���ת����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T GetSettings<T>(string json)
        {
            if (json.IsNotNullOrEmpty())
            {
                var setting = new JsonSerializerSettings();
                setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                setting.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
                setting.Converters.Add(new StringEnumConverter());
                return JsonConvert.DeserializeObject<T>(json, setting);
            }

            return default(T);
        }

        public static void GetProtocols()
        {
            if (AppSetting.ProtocolPath.IsNotNullOrEmpty())
            {
                var files = Directory.GetFiles(StartupPath + AppSetting.ProtocolPath);
                Protocols = new List<Protocol>();
                var json = string.Empty;
                Protocol protocol = null;
                foreach (var file in files)
                {
                    protocol = GetProtocolByFileName(file);
                    if (protocol != null && !Protocols.Any(x => x.Name == protocol.Name && x.Ver == protocol.Ver))
                    {
                        (Protocols as List<Protocol>).Add(protocol);
                    }
                }
            }
        }

        public static Protocol GetProtocol(string protocol, string ver = null)
        {
            try
            {
                return GetProtocolByName(protocol, ver) ?? GetProtocolByFileName(protocol) ?? GetSettings<Protocol>(protocol);
            }
            catch (Exception ex)
            {
                throw new Exception($"parameter name is not a valid protocol name, protocol file name or json format protocol config string! {ex.Message}");
            }
        }

        private static Protocol GetProtocolByName(string name, string ver = null)
        {
            if (name.IsNotNullOrEmpty() && Protocols.IsNotNullOrEmpty())
            {
                if (ver.IsNullOrEmpty())
                {
                    return Protocols.FirstOrDefault(x => x.Name == name);
                }
                else
                {
                    return Protocols.FirstOrDefault(x => x.Name == name && x.Ver == ver);
                }

            }

            return null;
        }

        private static Protocol GetProtocolByFileName(string fileName)
        {
            Protocol protocol = null;
            if (fileName.IsNotNullOrEmpty())
            {
                var fullName = fileName;
                if (!File.Exists(fullName))
                {
                    fullName = StartupPath + AppSetting.ProtocolPath + fileName;
                }

                if (File.Exists(fullName))
                {
                    var json = GetConfig(fileName);
                    if (json.IsNotNullOrEmpty())
                    {
                        protocol = GetSettings<Protocol>(json);
                        if (protocol != null)
                        {
                            protocol.Init();
                        }
                    }
                }
            }

            return protocol;
        }
    }

    /// <summary>
    /// App Setting in app.json
    /// </summary>
    public class AppSetting
    {
        public AppSetting()
        {
            LogPath = "/config/logs/";
            ProtocolPath = "/config/protocol/";
        }

        /// <summary>
        /// ��־Ŀ¼
        /// </summary>
        public string LogPath { get; set; }
        /// <summary>
        /// �ļ���ȡ��¼Ŀ¼
        /// </summary>
        public string ProtocolPath { get; set; }
        /// <summary>
        /// ���ڸ�ʽ
        /// </summary>
        public string DateFormat { get; set; }
        /// <summary>
        /// ʱ���ʽ
        /// </summary>
        public string DateTimeFormat { get; set; }
        /// <summary>
        /// Ĭ���ַ�����
        /// </summary>
        public string DefaultEncoding { get; set; }
    }

    public enum MessageFormat
    {
        Binary,
        Json,
        HexString
    }

    public enum ProtocolType
    {
        Socket,
        Tcp,
        Udp,
        Ftp
    }

    //
    // 摘要: from System.TypeCode, Change DBNull to Array
    //     Specifies the type of an object.
    public enum TypeCode
    {
        //
        // 摘要:
        //     A null reference.
        Empty = 0,
        //
        // 摘要:
        //     A general type representing any reference or value type not explicitly represented
        //     by another TypeCode.
        Object = 1,
        //
        // 摘要:
        //     A database null (column) value.
        //DBNull = 2,
        Array = 2,
        //
        // 摘要:
        //     A simple type representing Boolean values of true or false.
        Boolean = 3,
        //
        // 摘要:
        //     An integral type representing unsigned 16-bit integers with values between 0
        //     and 65535. The set of possible values for the System.TypeCode.Char type corresponds
        //     to the Unicode character set.
        Char = 4,
        //
        // 摘要:
        //     An integral type representing signed 8-bit integers with values between -128
        //     and 127.
        SByte = 5,
        //
        // 摘要:
        //     An integral type representing unsigned 8-bit integers with values between 0 and
        //     255.
        Byte = 6,
        //
        // 摘要:
        //     An integral type representing signed 16-bit integers with values between -32768
        //     and 32767.
        Int16 = 7,
        //
        // 摘要:
        //     An integral type representing unsigned 16-bit integers with values between 0
        //     and 65535.
        UInt16 = 8,
        //
        // 摘要:
        //     An integral type representing signed 32-bit integers with values between -2147483648
        //     and 2147483647.
        Int32 = 9,
        //
        // 摘要:
        //     An integral type representing unsigned 32-bit integers with values between 0
        //     and 4294967295.
        UInt32 = 10,
        //
        // 摘要:
        //     An integral type representing signed 64-bit integers with values between -9223372036854775808
        //     and 9223372036854775807.
        Int64 = 11,
        //
        // 摘要:
        //     An integral type representing unsigned 64-bit integers with values between 0
        //     and 18446744073709551615.
        UInt64 = 12,
        //
        // 摘要:
        //     A floating point type representing values ranging from approximately 1.5 x 10
        //     -45 to 3.4 x 10 38 with a precision of 7 digits.
        Single = 13,
        //
        // 摘要:
        //     A floating point type representing values ranging from approximately 5.0 x 10
        //     -324 to 1.7 x 10 308 with a precision of 15-16 digits.
        Double = 14,
        //
        // 摘要:
        //     A simple type representing values ranging from 1.0 x 10 -28 to approximately
        //     7.9 x 10 28 with 28-29 significant digits.
        Decimal = 15,
        //
        // 摘要:
        //     A type representing a date and time value.
        DateTime = 16,
        //
        // 摘要:
        //     A sealed class type representing Unicode character strings.
        String = 18
    }
}