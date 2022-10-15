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

        public string CommandPrefix { get; set; } = "!";

        public float GUIHeight { get; set; } = 370f;

        public float GUIWidth { get; set; } = 2f;

        public string TagHexColor { get; set; } = "#bdbdbd";

        public bool debugMode { get; set; } = false;


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