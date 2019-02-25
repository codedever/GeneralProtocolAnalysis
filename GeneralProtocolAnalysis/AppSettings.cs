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
}