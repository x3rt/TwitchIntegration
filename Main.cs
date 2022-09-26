using System;
using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Threading;
using ManagementScripts;
using MelonLoader;
using PropertiesScripts;
using SimulationScripts;
using SimulationScripts.BibiteScripts;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


[assembly: MelonInfo(typeof(TwitchIntegration.Main), "Twitch Integration", "1.0.0", "x3rt")]


namespace TwitchIntegration
{
    public class Main : MelonMod
    {
        
        public static bool IsEnabled;
        public static bool showGUI;
        
        private static object coRoutine;
        private static bool canRun = true;
        private static bool isRunning = false;
        private static string currentUser = String.Empty;


        public override void OnApplicationStart()
        {
            timeController = TimeController.Instance;
            Settings.Start();
            new Thread(() =>
            {
                for (;;)
                {
                    if (isRunning)
                    {
                        coRoutine = MelonCoroutines.Start(HighestGeneration());
                        LoggerInstance.Msg($"capNumber: {BibiteSpawner.capNumber}");
                    }

                    LoggerInstance.Msg($"{Settings.Instance.TwitchUsername}");
                    Settings.Load();
                    Thread.Sleep(5000);
                }
            }).Start();
        }


        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            LoggerInstance.Msg($"Scene {sceneName} with build index {buildIndex} has been loaded!");

            
            //buildIndex 0 = Loading Screen
            //buildIndex 1 = Main Menu
            //buildIndex 2 = Game
            if (buildIndex == 2)
            {
                // new Thread(() => { CollectGameObjects(); }).Start();
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
                showGUI = !showGUI;
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
            }
            if (Input.GetKeyDown(KeyCode.F4))
            {
            }
            
            if (Input.GetKeyDown(KeyCode.F6))
            {
                GameObject[] array3 = GameObject.FindGameObjectsWithTag("bibite");
                foreach (GameObject t in array3)
                {
                    Vector2 randomVector = new Vector2(Random.Range(-400f, 400f), Random.Range(-400f, 400f));
                    t.GetComponent<Rigidbody2D>().velocity = (10 * randomVector / Time.timeScale);
                }
            }

            if (Input.GetKeyDown(KeyCode.F8))
            {
                // BibiteSpawner.UpdateCapNumber(50);
                // LoggerInstance.Msg($"timeSlider.value: {timeController.timeSlider.value}");
                // LoggerInstance.Msg($"timeFactor: {timeController.timeFactor}");
                // LoggerInstance.Msg($"calc f to slider: {Mathf.Log(timeController.timeFactor, 5)}");
                // LoggerInstance.Msg($"calc slider to f: {Mathf.Pow(5f, timeController.timeSlider.value)}");
                // BibiteSpawner.UpdateCapNumber(100);
                
                
                
                // timeController.timeSlider.value = Mathf.Log(timeController.timeFactor + 0.1f, 5);
                //
                // timeController.timeSlider.value = Mathf.Log(timeController.timeFactor - 0.1f, 5);
                //
                //
                //
                //
                // timeController.timeSlider.value = Mathf.Log(4, 5); //convert float to slider value (4 in this case)
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                
            }
            
            if (Input.GetKeyDown(KeyCode.F10))
            {
                GameObject[] array3 = GameObject.FindGameObjectsWithTag("bibite");
                float num = 0f;
                GameObject gameObject = null;
                for (int i = 0; i < array3.Length; i++)
                {
                    array3[i].GetComponent<BibiteControl>().LayEgg();
                }
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
            GUI.TextField(new Rect(10, 10, 180, 20), currentUser, 25);
        }







        IEnumerator HighestGeneration()
        {
            LoggerInstance.Msg("Getting highest generation");
            // timeController = Object.FindObjectOfType<TimeController>();
            // yield return new WaitForSeconds(0.15f);
            // yield return new WaitForSeconds(0.15f);
            
            
            GameObject[] array3 = GameObject.FindGameObjectsWithTag("bibite");
            float num = 0f;
            GameObject gameObject = null;
            for (int i = 0; i < array3.Length; i++)
            {
                float num2 = (float)array3[i].GetComponent<BibiteGenes>().generation;
                if (num2 > num)
                {
                    num = num2;
                    gameObject = array3[i];
                }
            }
            if (gameObject != null)
            {
                float generation = gameObject.GetComponent<BibiteGenes>().generation;
                LoggerInstance.Msg($"Highest generation: {generation}");
            }
            
            yield return new WaitForSeconds(.5f);
        }



        private TimeController timeController;
        // private TimeController timeController;
        



    }
}