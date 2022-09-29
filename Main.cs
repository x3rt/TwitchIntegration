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
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


[assembly: MelonInfo(typeof(TwitchIntegration.Main), "Twitch Integration", "2.0.4", "x3rt")]


namespace TwitchIntegration
{
    public class Main : MelonMod
    {
        public static bool IsEnabled;
        public static bool showGUI;

        private static object coRoutine;
        private static object CameraCoroutine;
        private static object TagCoroutine;
        private static bool canRun = true;
        public static bool isRunning = false;

        public static MelonLogger.Instance? loggerInstance;
        public static bool isCinematic = false;


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
                        else
                        {
                            //if no message for 7 minutes
                            if (DateTime.Now.Subtract(a.lastMessageTime).TotalMinutes > 7)
                            {
                                if (Settings.Instance.debugMode)
                                    LoggerInstance.Msg("no message for 7 minutes, destroying");
                                Object.Destroy(a);
                                Thread.Sleep(1000);
                                GameObject.Find("__app")?.AddComponent<TwitchChat>();
                            }
                        }

                        if (GameObject.Find("__app")?.GetComponent<EventHandlers>() == null)
                        {
                            if (Settings.Instance.debugMode)
                                LoggerInstance.Msg("Event Handlers not found, creating new one");
                            GameObject.Find("__app")?.AddComponent<EventHandlers>();
                            if (Settings.Instance.debugMode)
                                LoggerInstance.Msg("Added event handlers");

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


        public override void OnLateUpdate()
        {
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
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
            }

            if (Input.GetKeyDown(KeyCode.F7))
            {
            }

            if (Input.GetKeyDown(KeyCode.F10))
            {
            }
        }

        public override void OnGUI()
        {
            if (showGUI)
            {
                DrawMenu();
            }
        }


        private void DrawMenu()
        {
            GUI.Box(new Rect(0, 0, 300, 500), "Twitch Integration");
            GUI.TextField(new Rect(10, 10, 180, 20), Settings.Instance.TwitchUsername, 25);
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