using System;
using System.Collections;
using System.IO;
using System.Threading;
using MelonLoader;
using TwitchIntegration.Config;
using TwitchIntegration.Loader;
using TwitchIntegration.UI;
using UnityEngine;
using UniverseLib;
using UniverseLib.Config;
using UniverseLib.Input;
using ConfigManager = TwitchIntegration.Config.ConfigManager;
using Object = UnityEngine.Object;


[assembly: MelonInfo(typeof(TwitchIntegration.Main), "Twitch Integration", "2.4.2", "x3rt")]


namespace TwitchIntegration
{
    public class Main : MelonMod, IExplorerLoader
    {
        public static bool IsEnabled;

        private static object coRoutine;
        private static object CameraCoroutine;
        private static object TagCoroutine;
        private static bool canRun = true;
        public static bool isRunning = false;

        public static MelonLogger.Instance? loggerInstance;
        public static bool isCinematic = false;
        public static int cinematicInterval = 0;
        public static int tempnum = 15;
        public static int tempnum2 = 15;

        public static Rect windowRect;
        public static IExplorerLoader Loader { get; private set; }
        public static Main Instance { get; private set; }


        public override void OnApplicationStart()
        {
            Instance = this;
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
                            if (ConfigManager.Debug_Mode.Value)
                                LoggerInstance.Msg("Statistics not found");
                            GameObject.Find("__app")?.AddComponent<Statistics>();
                            if (ConfigManager.Debug_Mode.Value)
                                LoggerInstance.Msg("Added statistics");
                        }
                        
                        

                        if (GameObject.Find("__app")?.GetComponent<EventHandlers>() == null)
                        {
                            if (ConfigManager.Debug_Mode.Value)
                                LoggerInstance.Msg("Event Handlers not found");
                            GameObject.Find("__app")?.AddComponent<EventHandlers>();
                        }
                        
                        // if (GameObject.Find("__app")?.GetComponent<x3rtGUI>() == null)
                        // {
                        //     if (ConfigManager.Debug_Mode.Value)
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
                        //         if (ConfigManager.Debug_Mode.Value)
                        //             LoggerInstance.Msg("Adding text for: " + ob.GetInstanceID());
                        //         _ = ob.AddComponent(typeof(BiBiteMono));
                        //         if (ConfigManager.Debug_Mode.Value)
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
                            if (ConfigManager.Debug_Mode.Value)
                                LoggerInstance.Msg("Twitch Chat not found, adding");
                            GameObject.Find("__app")?.AddComponent<TwitchChat>();
                            if (ConfigManager.Debug_Mode.Value)
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
            
            
            _configHandler = new MelonLoaderConfigHandler();
            Init(this);
            Directory.CreateDirectory(ExplorerFolder);
            Config.ConfigManager.Init(this.ConfigHandler);
            Universe.Init(ConfigManager.Startup_Delay_Time.Value, UIManager.InitUI, Log, new UniverseLibConfig() );
        }

        public override void OnApplicationQuit()
        {
            Settings.Save();
        }


        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (ConfigManager.Debug_Mode.Value)
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

        public override void OnLateUpdate()
        {
            
            
            if (InputManager.GetKeyDown(ConfigManager.Navbar_Toggle.Value))
            {
                UIManager.ShowNavbar = !UIManager.ShowNavbar;
            }
            
            if (InputManager.GetKeyDown(ConfigManager.TwitchChat_Reconnect.Value))
            {
                TwitchChat? a = GameObject.Find("__app")?.GetComponent<TwitchChat>();
                Object.Destroy(a);

                if (a != null)
                    a.Connect();
                else
                    GameObject.Find("__app")?.AddComponent<TwitchChat>();
            }
            
            


            if (Input.GetKeyDown(KeyCode.F2))
            {
                // showGUI = !showGUI;
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                // There was stuff here before for testing purposes
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                // Settings.Load();
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                // if (Statistics.Instance == null)
                // {
                //     LoggerInstance.Msg("was null");
                // }
                // else
                // {
                //     StatisticsData.Save();
                // }
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
            if (ConfigManager.Debug_Mode.Value)
                LoggerInstance.Msg("Getting highest generation");

            Tools.GetHighestGeneration();

            yield return new WaitForSeconds(.5f);
        }
        
        private static void Log(object message, LogType logType)
        {
            string log = message?.ToString() ?? "";

            // LogPanel.Log(log, logType);

            switch (logType)
            {
                case LogType.Assert:
                case LogType.Log:
                    MelonLogger.Msg(log);
                    break;

                case LogType.Warning:
                    MelonLogger.Warning(log);
                    break;

                case LogType.Error:
                case LogType.Exception:
                    MelonLogger.Error(log);
                    break;
            }
        }
        
        
        public static void Log(object message)
            => Log(message, LogType.Log);

        public static void LogWarning(object message)
            => Log(message, LogType.Warning);

        public static void LogError(object message)
            => Log(message, LogType.Error);

        public static void Init(IExplorerLoader loader)
        {
            if (Loader != null)
                throw new Exception("TwitchIntegration is already loaded.");
            
            Loader = loader;
            Log($"Twitch Integration {Instance.Info.Version} initializing...");
            
        }


        public string ExplorerFolderName => "TwitchIntegration";
        public string ExplorerFolderDestination => MelonHandler.ModsDirectory;
        public static string ExplorerFolder => Path.Combine(Loader.ExplorerFolderDestination, Loader.ExplorerFolderName);

        public string UnhollowedModulesFolder => Path.Combine(
            Path.GetDirectoryName(MelonHandler.ModsDirectory),
            Path.Combine("MelonLoader", "Managed"));

        public ConfigHandler ConfigHandler => _configHandler;
        public MelonLoaderConfigHandler _configHandler;

        public Action<object> OnLogMessage => LoggerInstance.Msg;
        public Action<object> OnLogWarning => LoggerInstance.Warning;
        public Action<object> OnLogError   => LoggerInstance.Error;
    }
}