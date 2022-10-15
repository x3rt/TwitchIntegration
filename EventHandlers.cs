using System;
using System.Collections;
using UnityEngine;
using ManagementScripts;


namespace TwitchIntegration
{
    public class EventHandlers : MonoBehaviour
    {
        private int xCam = 0;


        private void Awake()
        {
        }


        private IEnumerator CameraCoroutine()
        {
            for (;;)
            {
                yield return new WaitForSecondsRealtime(1f);


                if (Main.isRunning)
                {
                    if (Main.isCinematic)
                    {
                        if (Main.cinematicInterval > 0)
                        {
                            xCam++;
                            if (xCam >= Main.cinematicInterval)
                            {
                                var a = Tools.GetRandomEntity(notThis: UserControl.Instance.target);
                                if (a == null) yield break;
                                UserControl.Instance.SelectTarget(a);

                                xCam = 0;
                            }
                        }
                    }
                }
            }
        }


        private IEnumerator BibiteCoroutine()
        {
            GameObject? ob;
            for (;;)
            {
                yield return new WaitForSecondsRealtime(3f);
                if (!Main.isRunning) continue;
                foreach (Transform transform1 in WorldObjectsSpawner.Instance.bibiteHolder.transform)
                {
                    ob = transform1.gameObject;
                    if (ob.tag != "bibite")
                        continue;
                    BiBiteMono? bb = ob.GetComponent<BiBiteMono>();
                    if (bb == null)
                    {
                        _ = ob.AddComponent(typeof(BiBiteMono));

                    }




                }
            }
        }


        private void Start()
        {
            Main.loggerInstance?.Msg("EventHandlers Started");
            StartCoroutine(CameraCoroutine());
            StartCoroutine(BibiteCoroutine());
        }

        private void OnDestroy()
        {
            Main.loggerInstance?.Msg("Destroying EventHandlers");
            StopCoroutine(CameraCoroutine());
            StopCoroutine(BibiteCoroutine());
        }


        private void Update()
        {
            CinemaCam();
        }

        void CinemaCam()
        {
            if (!Main.isRunning) return;
            if (!Main.isCinematic) return;
            try
            {
                var a = Tools.GetClosestEntity();
                if (a == null) return;
                UserControl.Instance.SelectTarget(a);
            }
            catch (Exception e)
            {
                Main.loggerInstance?.Error(e.Message);
            }
        }
    }
}