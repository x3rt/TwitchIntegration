using System;
using System.IO;
using Newtonsoft.Json;
using SimulationScripts;
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
        public DateTime? lastSave { get; set; } = DateTime.Now;

        private void Start()
        {
            StatisticsData.Load();
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

            if (lastSave == null || (DateTime.Now - lastSave.Value).TotalSeconds > 60)
            {
                Settings.Save();
                lastSave = DateTime.Now;
            }
        }


        private void updateHighestPopulation()
        {
            if (Instance == null)
            {
                Main.loggerInstance?.Msg("Instance is null");
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
                Main.loggerInstance?.Msg("Instance is null");
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
                Main.loggerInstance?.Msg("Instance is null");
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
            Settings.Save();
        }


        public void Load()
        {
            Instance = this;
            Instance.AllTimeHighestPopulation = StatisticsData.Instance.AllTimeHighestPopulation;
            Instance.AllTimeHighestGeneration = StatisticsData.Instance.AllTimeHighestGeneration;
            Instance.AllTimeHighestAge = StatisticsData.Instance.AllTimeHighestAge;
        }
    }

    public class StatisticsData
    {
        private static string path = "TwitchIntegration-Data.json";

        public static StatisticsData Instance { get; set; }

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


        public static void Load()
        {
            if (!File.Exists(path))
            {
                File.WriteAllText(path, new StatisticsData().ToJson());
            }

            StatisticsData.Instance = JsonConvert.DeserializeObject<StatisticsData>(File.ReadAllText(path));
        }

        public static void Save()
        {
            //save to json file
            var json = Instance.ToJson();
            File.WriteAllText(path, json);
        }

        public static void Start()
        {
            Instance = new StatisticsData();
            Load();
        }
    }
}