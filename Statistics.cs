using System;
using System.IO;
using Newtonsoft.Json;
using TwitchIntegration.Config;
using UnityEngine;

namespace TwitchIntegration
{
    public class Statistics : MonoBehaviour
    {
        public static Statistics? Instance { get; set; }
        public int SessionHighestGeneration { get; set; } = 0;
        public int CurrentHighestGeneration { get; set; } = 0;
        public int AllTimeHighestGeneration { get; set; } = 0;

        public int SessionHighestPopulation { get; set; } = 0;
        public int AllTimeHighestPopulation { get; set; } = 0;

        public int CurrentHighestAge { get; set; } = 0;
        public int SessionHighestAge { get; set; } = 0;
        public int AllTimeHighestAge { get; set; } = 0;


        public DateTime? lastRefresh { get; set; }
        public DateTime lastSave { get; set; } = DateTime.Now;

        private void Start()
        {
            StatisticsData.Start();
            Load();
            Update();
        }


        private void Update()
        {
            if (lastRefresh == null || (DateTime.Now - lastRefresh.Value).TotalSeconds > 5)
            {
                updateHighestGeneration();

                updateHighestPopulation();

                updateHighestAge();

                lastRefresh = DateTime.Now;
            }

            if ((DateTime.Now - lastSave).TotalSeconds > 60)
            {
                StatisticsData.Save();
                lastSave = DateTime.Now;
            }
        }


        private void updateHighestPopulation()
        {
            if (Instance == null)
            {
                if (ConfigManager.Debug_Mode.Value)
                    Main.loggerInstance?.Msg("Statistics Instance is null");
                return;
            }

            var a = Tools.GetBibiteCount();

            if (a > Instance.SessionHighestPopulation)
            {
                Instance.SessionHighestPopulation = a;
            }

            if (a > Instance.AllTimeHighestPopulation)
            {
                Instance.AllTimeHighestPopulation = a;
            }
        }

        private void updateHighestGeneration()
        {
            if (Instance == null)
            {
                if (ConfigManager.Debug_Mode.Value)
                    Main.loggerInstance?.Msg("Statistics Instance is null");
                return;
            }

            var a = Tools.GetHighestGeneration();

            if (a > Instance.SessionHighestGeneration)
            {
                Instance.SessionHighestGeneration = a;
            }

            if (a > Instance.AllTimeHighestGeneration)
            {
                Instance.AllTimeHighestGeneration = a;
            }

            Instance.CurrentHighestGeneration = a;
        }


        private void updateHighestAge()
        {
            if (Instance == null)
            {
                if (ConfigManager.Debug_Mode.Value)
                    Main.loggerInstance?.Msg("Statistics Instance is null");
                return;
            }

            var a = Tools.GetHighestAge();

            if (a > Instance.SessionHighestAge)
            {
                Instance.SessionHighestAge = a;
            }

            if (a > Instance.AllTimeHighestAge)
            {
                Instance.AllTimeHighestAge = a;
            }

            Instance.CurrentHighestAge = a;
        }

        private void OnDestroy()
        {
            StatisticsData.Save();
            Destroy(this);
        }


        public void Load()
        {
            Instance = this;
            if (StatisticsData.Instance != null)
            {
                Instance.AllTimeHighestPopulation = StatisticsData.Instance.AllTimeHighestPopulation;
                Instance.AllTimeHighestGeneration = StatisticsData.Instance.AllTimeHighestGeneration;
                Instance.AllTimeHighestAge = StatisticsData.Instance.AllTimeHighestAge;
            }
        }
    }

    public class StatisticsData
    {
        private static string path = "TwitchIntegration-Data.json";

        public static StatisticsData? Instance { get; set; }

        public int AllTimeHighestGeneration { get; set; } = 0;
        public int AllTimeHighestPopulation { get; set; } = 0;
        public int AllTimeHighestAge { get; set; } = 0;


        public string ToJson()
        {
            var sw = new StringWriter();

            using (var writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();

                writer.WritePropertyName("AllTimeHighestGeneration");
                writer.WriteValue(AllTimeHighestGeneration);

                writer.WritePropertyName("AllTimeHighestPopulation");
                writer.WriteValue(AllTimeHighestPopulation);

                writer.WritePropertyName("AllTimeHighestAge");
                writer.WriteValue(AllTimeHighestAge);

                writer.WriteEndObject();
            }

            return sw.ToString();
        }
        public static string getData()
        {
            var sw = new StringWriter();

            using (var writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();

                writer.WritePropertyName("AllTimeHighestGeneration");
                writer.WriteValue(Statistics.Instance == null ? 0 : Statistics.Instance.AllTimeHighestGeneration);

                writer.WritePropertyName("AllTimeHighestPopulation");
                writer.WriteValue(Statistics.Instance == null ? 0 : Statistics.Instance.AllTimeHighestPopulation);

                writer.WritePropertyName("AllTimeHighestAge");
                writer.WriteValue(Statistics.Instance == null ? 0 : Statistics.Instance.AllTimeHighestAge);

                writer.WriteEndObject();
            }

            return sw.ToString();
        }


        public static void Load()
        {
            
            if (Instance == null)
            {
                Start(false);
            }
            
            if (!File.Exists(path))
            {
                File.WriteAllText(path, Instance?.ToJson());
            }

            StatisticsData.Instance = JsonConvert.DeserializeObject<StatisticsData>(File.ReadAllText(path));
        }

        public static void Save()
        {
            //save to json file
            if (Instance == null)
            {
                Start(false);
            }
            var json = getData();
            File.WriteAllText(path, json);
        }

        public static void Start(bool load = true)
        {
            Instance = new StatisticsData();
            if(load)
                Load();
        }
    }
}