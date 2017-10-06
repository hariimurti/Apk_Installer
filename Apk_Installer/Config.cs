using Newtonsoft.Json;
using System;
using System.IO;

namespace Apk_Installer
{
    internal class Config
    {
        private class Data
        {
            public string ip_address { get; set; }
            public int port { get; set; }
            public bool check_ext { get; set; }
        }

        private static string LOCAL_DATA = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "APK Installer");
        private static string CONFIG_FILE = Path.Combine(LOCAL_DATA, "config.json");
        private Data json_config;

        private static JsonSerializerSettings JSON_SETTINGS = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Include,
        };

        public Config()
        {
            string raw_config;

            if (!Directory.Exists(LOCAL_DATA))
                Directory.CreateDirectory(LOCAL_DATA);

            if (File.Exists(CONFIG_FILE))
            {
                raw_config = File.ReadAllText(CONFIG_FILE);
            }
            else
            {
                var data = new
                {
                    ip_address = "192.168.1.101",
                    port = 5555,
                    check_ext = true
                };
                raw_config = JsonConvert.SerializeObject(data, JSON_SETTINGS);
                File.WriteAllText(CONFIG_FILE, raw_config);
            }

            json_config = JsonConvert.DeserializeObject<Data>(raw_config);
        }

        public void Save()
        {
            string raw_config = JsonConvert.SerializeObject(json_config, JSON_SETTINGS);
            File.WriteAllText(CONFIG_FILE, raw_config);
        }

        public bool getCheckExt()
        {
            return json_config.check_ext;
        }

        public string getIPaddress()
        {
            return json_config.ip_address;
        }

        public int getPort()
        {
            return json_config.port;
        }

        public void setCheckExt(bool value)
        {
            json_config.check_ext = value;
            this.Save();
        }

        public void setAdbWifi(string newIpAddress, int newPort)
        {
            json_config.ip_address = newIpAddress;
            json_config.port = newPort;
            this.Save();
        }
    }
}