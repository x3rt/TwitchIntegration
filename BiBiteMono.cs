using System;
using System.Collections;
using ManagementScripts;
using SimulationScripts.BibiteScripts;
using TMPro;
using UIScripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TwitchIntegration
{
    public class BiBiteMono : MonoBehaviour
    {
        public TextMeshPro? text; //tez
        public string? textToDisplay;
        public float offset = 10f;
        public DateTime lastSynced = DateTime.Now.Subtract(TimeSpan.FromSeconds(Random.Range(0f, 5f)));

        public BibiteGenes? bibiteGenes;
        public BibiteBody? bibiteBody;
        public BibiteControl? BibiteControl;

        public bool initialized = false;
        public bool currentlySyncing = false;
        public bool firstSetup = true;


        public void Start()
        {
            Main.loggerInstance?.Msg("Starting BibiteMono:" + gameObject.GetInstanceID());
            Invoke(nameof(Setup), 1f);
        }

        private void Setup()
        {
            try
            {
                GameObject o = new GameObject();

                Instantiate(o, gameObject.transform);

                text = o.AddComponent<TextMeshPro>();
                var position = gameObject.transform.position;
                text.transform.position = new Vector3(position.x, position.y + offset, 0);
                text.color = Color.white;
                text.alignment = TextAlignmentOptions.Center;
                text.outlineWidth = 5f;
                text.outlineColor = Color.green;
            }
            catch (Exception e)
            {
                Main.loggerInstance?.Msg(e.Message);
            }

            
            if (Settings.Instance.debugMode)
                Main.loggerInstance?.Msg("Setting Up:" + gameObject.GetInstanceID());

            gameObject.TryGetComponent(out bibiteGenes);
            if (Settings.Instance.debugMode)
                Main.loggerInstance?.Msg("Got BibiteGenes:" + gameObject.GetInstanceID());
            gameObject.TryGetComponent(out bibiteBody);
            if (Settings.Instance.debugMode)
                Main.loggerInstance?.Msg("Got BibiteBody:" + gameObject.GetInstanceID());
            gameObject.TryGetComponent(out BibiteControl);
            if (Settings.Instance.debugMode)
                Main.loggerInstance?.Msg("Got BibiteControl:" + gameObject.GetInstanceID());

            if (bibiteGenes != null && bibiteBody != null && BibiteControl != null)
            {
                initialized = true;
            }

            firstSetup = false;
            if (Settings.Instance.debugMode)
                Main.loggerInstance?.Msg("Added text for: " + gameObject.GetInstanceID());
        }

        public void Update()
        {
            if (firstSetup) return;
            if (!initialized)
            {
                gameObject.TryGetComponent(out bibiteGenes);
                if (Settings.Instance.debugMode)
                    Main.loggerInstance?.Msg("Got BibiteGenes:" + gameObject.GetInstanceID());
                gameObject.TryGetComponent(out bibiteBody);
                if (Settings.Instance.debugMode)
                    Main.loggerInstance?.Msg("Got BibiteBody:" + gameObject.GetInstanceID());
                gameObject.TryGetComponent(out BibiteControl);
                if (Settings.Instance.debugMode)
                    Main.loggerInstance?.Msg("Got BibiteControl:" + gameObject.GetInstanceID());
            }

            ;
            if (text == null) return;
            var o = gameObject;

            if (textToDisplay == null || ((DateTime.Now - lastSynced).TotalSeconds > 7f && !currentlySyncing))
            {
                currentlySyncing = true;
                lastSynced = DateTime.Now;
                if (Settings.Instance.debugMode)
                    Main.loggerInstance?.Msg("Updating text");
                var az = bibiteBody!.bodyLength;
                offset = az + 2f;
                if (Settings.Instance.debugMode)
                    Main.loggerInstance?.Msg("Got offset: " + offset);
                textToDisplay = bibiteGenes!.speciesTag;
                
                // if (Settings.Instance.debugMode)
                //     if (textToDisplay == "")
                //         textToDisplay = "Tagless";
                if (Settings.Instance.debugMode)
                    Main.loggerInstance?.Msg("Got text: " + textToDisplay);
                currentlySyncing = false;
            }

            Vector3 position = o.transform.position;
            text.transform.position = new Vector3(position.x, position.y + offset, 0);
            text.text = textToDisplay;
        }

        private void OnDestroy()
        {
            Destroy(text);
            Destroy(this);
        }
    }
}