using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace TwitchIntegration
{
    public class Settings
    {
        
        private static string path = "TwitchIntegration.json";
        
        public static Settings Instance { get; set; }
        
        
        public string TwitchUsername { get; set; } = "username";
        
        public float allTimeHighestGeneration { get; set; } = 0;





        public string ToJson()
        {
            var sw = new StringWriter();

            using(var writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();

                writer.WritePropertyName(nameof(TwitchUsername));
                writer.WriteValue(TwitchUsername!);

                writer.WritePropertyName(nameof(allTimeHighestGeneration));
                writer.WriteValue(allTimeHighestGeneration);
                
                writer.WriteEndObject();
            }

            return sw.ToString();
        }






        public static void Load()
        {
            if (!File.Exists(path))
            {
                File.WriteAllText(path, new Settings().ToJson());
                
                
            }
            
            Settings.Instance = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(path));
            
            
            
        }
        
        public static void Save()
        {
            //save to json file
            var json = Instance.ToJson();
            File.WriteAllText(path, json);
            
        }

        public static void Start()
        {
            
            Instance = new Settings();
            Load();
        }
        
        
        
        
        
    }
}