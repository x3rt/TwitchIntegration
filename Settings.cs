using System.IO;
using Newtonsoft.Json;

namespace TwitchIntegration
{
    public class Settings
    {
        private static string path = "TwitchIntegration.json";

        public static Settings Instance { get; set; }
        public string TwitchUsername { get; set; } = "username";

        public string TwitchOAuth { get; set; } = "oauth:token";


        public static void Load()
        {
            if (!File.Exists(path))
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(new Settings(), Formatting.Indented));
            }

            Instance = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(path));
            Save();
        }

        public static void Save()
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(Instance, Formatting.Indented));
        }

        public static void Start()
        {
            Instance = new Settings();
            Load();
        }
    }
}