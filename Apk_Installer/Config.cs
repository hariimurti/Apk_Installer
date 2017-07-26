using Newtonsoft.Json;
using System.IO;

namespace Apk_Installer
{
    class Config
    {
        class Data
        {
            public string ip_address { get; set; }
            public int port { get; set; }
            public bool register_apk { get; set; }
        }

        private static string CONFIG_FILE = "config.ini";
        private Data json_config;
        private static JsonSerializerSettings JSON_SETTINGS = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Include,
        };

        public Config()
        {
            string raw_config;

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
                    register_apk = true
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

        public bool getRegisterApk()
        {
            return json_config.register_apk;
        }

        public string getIPaddress()
        {
            return json_config.ip_address;
        }

        public int getPort()
        {
            return json_config.port;
        }

        public void setRegisterApk(bool value)
        {
            json_config.register_apk = value;
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
