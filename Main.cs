using System;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Threading;
using System.Threading.Tasks;
using ManagementScripts;
using MelonLoader;
using PropertiesScripts;
using SimulationScripts;
using SimulationScripts.BibiteScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


[assembly: MelonInfo(typeof(TwitchIntegration.Main), "Twitch Integration", "2.4.0", "x3rt")]


namespace TwitchIntegration
{
    public class Main : MelonMod
    {
        public static bool IsEnabled;
        public static bool showGUI = true;

        private static object coRoutine;
        private static object CameraCoroutine;
        private static object TagCoroutine;
        private static bool canRun = true;
        public static bool isRunning = false;

        public static MelonLogger.Instance? loggerInstance;
        public static bool isCinematic = false;
        public static int cinematicInterval = 0;

        public static Statistics statistics = new Statistics();
        public static int tempnum = 15;
        public static int tempnum2 = 15;

        public static bool isClicked = false;

        public static Rect windowRect;


        public override void OnApplicationStart()
        {
            loggerInstance = LoggerInstance;
            Settings.Start();
            new Thread(() =>
            {
                for (;;)
                {
                    if (isRunning)
                    {
                        // coRoutine = MelonCoroutines.Start(HighestGeneration());
                        // LoggerInstance.Msg($"capNumber: {BibiteSpawner.capNumber}");


                        if (GameObject.Find("__app")?.GetComponent<Statistics>() == null)
                        {
                            if (Settings.Instance.debugMode)
                                LoggerInstance.Msg("Statistics not found");
                            GameObject.Find("__app")?.AddComponent<Statistics>();
                            if (Settings.Instance.debugMode)
                                LoggerInstance.Msg("Added statistics");
                        }
                        
                        

                        if (GameObject.Find("__app")?.GetComponent<EventHandlers>() == null)
                        {
                            if (Settings.Instance.debugMode)
                                LoggerInstance.Msg("Event Handlers not found");
                            GameObject.Find("__app")?.AddComponent<EventHandlers>();
                        }
                        
                        // if (GameObject.Find("__app")?.GetComponent<x3rtGUI>() == null)
                        // {
                        //     if (Settings.Instance.debugMode)
                        //         LoggerInstance.Msg("x3 GUI not found");
                        //     GameObject.Find("__app")?.AddComponent<x3rtGUI>();
                        // }


                        // GameObject[] array3 = GameObject.FindGameObjectsWithTag("bibite");
                        // int num = 0;
                        // foreach (GameObject ob in array3)
                        // {
                        //     BiBiteMono? bb = ob.GetComponent<BiBiteMono>();
                        //     if (bb == null)
                        //     {
                        //         if (Settings.Instance.debugMode)
                        //             LoggerInstance.Msg("Adding text for: " + ob.GetInstanceID());
                        //         _ = ob.AddComponent(typeof(BiBiteMono));
                        //         if (Settings.Instance.debugMode)
                        //             LoggerInstance.Msg("After AddComponent for: " + ob.GetInstanceID());
                        //             
                        //     }
                        // }
                    }


                    try
                    {
                        var a = GameObject.Find("__app")?.GetComponent<TwitchChat>();
                        if (a == null)
                        {
                            if (Settings.Instance.debugMode)
                                LoggerInstance.Msg("Twitch Chat not found, adding");
                            GameObject.Find("__app")?.AddComponent<TwitchChat>();
                            if (Settings.Instance.debugMode)
                                LoggerInstance.Msg("Added Twitch Chat");
                        }
                    }
                    catch (Exception e)
                    {
                        LoggerInstance.Msg(e.Message);
                    }

                    Thread.Sleep(5000);
                }
            }).Start();
        }

        public override void OnApplicationQuit()
        {
            Settings.Save();
        }


        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (Settings.Instance.debugMode)
                LoggerInstance.Msg($"Scene {sceneName} with build index {buildIndex} has been loaded!");


            //buildIndex 0 = Loading Screen
            //buildIndex 1 = Main Menu
            //buildIndex 2 = Game
            if (buildIndex == 2)
            {
                isRunning = true;
            }
            else
            {
                isRunning = false;
            }
        }

        //ongui


        public override void OnGUI()
        {
            if (showGUI)
            {
                // int yo = 20;
                // int xo = 0;
                // GUI.Box(new Rect(10 + xo, 10 + yo, width, tempnum), "Twitch Integration");
                //
                // GUI.Label(new Rect(20 + xo, 40 + yo, width, 20), "Cinematic Camera: " +
                //                                                  (isCinematic ? "Enabled" : "Disabled"));
                // GUI.Label(new Rect(20 + xo, 60 + yo, width, 20),
                //     "All Time highest generation: " + Statistics.AllTimeHighestGeneration);
                // GUI.Label(new Rect(20 + xo, 80 + yo, width, 20),
                //     "Session highest generation: " + Statistics.SessionHighestGeneration);
                // GUI.Label(new Rect(20 + xo, 100 + yo, width, 20),
                //     "Current highest generation: " + Statistics.CurrentHighestGeneration);
                // GUI.Label(new Rect(20 + xo, 120 + yo, width, 20),
                //     "All Time highest population: " + Statistics.AllTimeHighestPopulation);
                // GUI.Label(new Rect(20 + xo, 140 + yo, width, 20),
                //     "Session highest population: " + Statistics.SessionHighestPopulation);
                // GUI.Label(new Rect(20 + xo, 160 + yo, width, 20),
                //     "All Time highest age: " + Statistics.AllTimeHighestAge);
                // GUI.Label(new Rect(20 + xo, 180 + yo, width, 20),
                //     "Session highest age: " + Statistics.SessionHighestAge);
                // GUI.Label(new Rect(20 + xo, 200 + yo, width, 20),
                //     "Current highest age: " + Statistics.CurrentHighestAge);

                if (isClicked)
                {
                    Settings.Instance.GUIHeight = Screen.height - Input.mousePosition.y;
                    Settings.Instance.GUIWidth = Input.mousePosition.x;
                }

                float y = Settings.Instance.GUIHeight;
                float x = Settings.Instance.GUIWidth;
                int width = 210;


                windowRect = new Rect(x, y, width, 235);
                GUI.Box(windowRect, "<color=#9046ff>Twitch Integration </color>");
                
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.alignment = TextAnchor.MiddleCenter;
                x -= 8;
                GUI.Label(new Rect(x, y, width, 50), $"<size=12><color=#23f753>by x3rt</color></size>", style);
                x += 8;
                y += 30;
                x += 10;
                GUI.Label(new Rect(x, y, width, 50), "Cinematic Camera: " + (isCinematic ? "Enabled" : "Disabled"));
                y += 20;
                GUI.Label(new Rect(x, y, width, 50), "Cinematic Auto-switch: " + (cinematicInterval > 0 ? $"{cinematicInterval}s" : "Disabled"));
                y += 20;
                if (Statistics.Instance != null)
                {
                    // Main.loggerInstance?.Msg("Statistics is not null");
                    GUI.Label(new Rect(x, y, width, 50),
                        "All Time highest generation: " + Statistics.Instance.AllTimeHighestGeneration);
                    y += 20;
                    GUI.Label(new Rect(x, y, width, 50),
                        "Session highest generation: " + Statistics.Instance.SessionHighestGeneration);
                    y += 20;
                    GUI.Label(new Rect(x, y, width, 50),
                        "Current highest generation: " + Statistics.Instance.CurrentHighestGeneration);
                    y += 20;
                    GUI.Label(new Rect(x, y, width, 50),
                        "All Time highest population: " + Statistics.Instance.AllTimeHighestPopulation);
                    y += 20;
                    GUI.Label(new Rect(x, y, width, 50),
                        "Session highest population: " + Statistics.Instance.SessionHighestPopulation);
                    y += 20;
                    GUI.Label(new Rect(x, y, width, 50),
                        "All Time highest age: " + Statistics.Instance.AllTimeHighestAge);
                    y += 20;
                    GUI.Label(new Rect(x, y, width, 50),
                        "Session highest age: " + Statistics.Instance.SessionHighestAge);
                    y += 20;
                    GUI.Label(new Rect(x, y, width, 50),
                        "Current highest age: " + Statistics.Instance.CurrentHighestAge);
                }
            }
        }

        public override void OnLateUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Input.mousePosition.x > windowRect.xMin && Input.mousePosition.x < windowRect.xMax &&
                    Screen.height - Input.mousePosition.y > windowRect.yMin &&
                    Screen.height - Input.mousePosition.y < windowRect.yMax)
                {
                    isClicked = true;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isClicked = false;
            }
            
            


            if (Input.GetKeyDown(KeyCode.F2))
            {
                // showGUI = !showGUI;
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                // There was stuff here before for testing purposes
                TwitchChat? a = GameObject.Find("__app")?.GetComponent<TwitchChat>();
                Object.Destroy(a);

                if (a != null)
                    a.Connect();
                else
                    GameObject.Find("__app")?.AddComponent<TwitchChat>();
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                Settings.Load();
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                if (Statistics.Instance == null)
                {
                    LoggerInstance.Msg("was null");
                }
                else
                {
                    StatisticsData.Save();
                }
                // tempnum -= 1;
                // LoggerInstance.Msg($"temp1: {tempnum}");
            }

            if (Input.GetKeyDown(KeyCode.F7))
            {
            }

            if (Input.GetKeyDown(KeyCode.F10))
            {
                // tempnum2 -= 1;
                //
                // LoggerInstance.Msg($"temp2: {tempnum2}");
            }

            if (Input.GetKeyDown(KeyCode.F11))
            {
                // tempnum2 += 1;
                //
                // LoggerInstance.Msg($"temp2: {tempnum2}");
            }
        }

        // public override void OnGUI()
        // {
        //     if (showGUI)
        //     {
        //         DrawMenu();
        //     }
        // }


        private void DrawMenu()
        {
            GUI.Box(new Rect(0, 0, 300, 500), "Twitch Integration");
            GUI.Label(new Rect(0, 100, 300, 500), "TEst Integration");
        }

        private IEnumerator HighestGeneration()
        {
            if (Settings.Instance.debugMode)
                LoggerInstance.Msg("Getting highest generation");

            Tools.GetHighestGeneration();

            yield return new WaitForSeconds(.5f);
        }
    }
}