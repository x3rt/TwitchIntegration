using System;
using UnityEngine;
using ManagementScripts;


namespace TwitchIntegration
{

    public class EventHandlers : MonoBehaviour
    {
        private void Awake()
        {
        }

        private void OnDestroy()
        {
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