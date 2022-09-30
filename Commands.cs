using ManagementScripts;
using SettingScripts;
using SimulationScripts;
using SimulationScripts.BibiteScripts;
using UnityEngine;

namespace TwitchIntegration
{
    public static class Commands
    {

        public static void ReloadSettings()
        {
            Main.loggerInstance?.Msg("Reloading");
            Settings.Load();
            Main.loggerInstance?.Msg("Reloaded");
        }
        
        
        public static void layAll()
        {
            GameObject[] array3 = GameObject.FindGameObjectsWithTag("bibite");
            foreach (GameObject t in array3)
            {
                if (t.TryGetComponent(out BibiteControl bc))
                {
                    bc.LayEgg();
                }
            }
        }
        
        public static void lay(GameObject bibite)
        {
            if (bibite.TryGetComponent(out BibiteControl bc))
            {
                bc.LayEgg();
            }
            
        }
        
        public static void pushAll(int factor = 10, float min = -1f, float max = 1f)
        {
            GameObject[] array3 = GameObject.FindGameObjectsWithTag("bibite");
            foreach (GameObject t in array3)
            {
                if (t.TryGetComponent(out Rigidbody2D rb))
                {
                    Vector2 randomVector = new Vector2(Random.Range(min, max), Random.Range(min, max));
                    rb.velocity = (factor * 4000 * randomVector / Time.timeScale);
                }
            }
        }
        
        public static void push(GameObject entity, int factor = 10, float min = -1f, float max = 1f)
        {
            if (entity.TryGetComponent(out Rigidbody2D rb))
            {
                Vector2 randomVector = new Vector2(Random.Range(min, max), Random.Range(min, max));
                rb.velocity = (factor * 4000 * randomVector / Time.timeScale);
            }
        }

        public static void UpdateMaximumBiBites(int max)
        {
            BibiteSpawner.UpdateCapNumber(max);
        }

        public static void SetSpeed(float speed)
        {
            TimeController.Instance.timeSlider.value = Tools.FactorToSlider(speed);
        }

        public static void SelectRandomEntity()
        {
            GameObject[] gameObjectsWithTag = GameObject.FindGameObjectsWithTag("bibite");
            if (gameObjectsWithTag.Length != 0)
                UserControl.Instance.SelectTarget(gameObjectsWithTag[Random.Range(0, gameObjectsWithTag.Length)]);
        }

        public static void ZoomIn()
        {
            Zoom(1);

        }
        
        public static void ZoomOut()
        {
           Zoom(-1);
        }

        public static void Zoom(float zoom)
        {
            var cam = Camera.main;
            // float z = Tools.MinMaxDefault(main.position.z - zoom, 1, 100);
            // if (main != null) main.position = new Vector3(main.position.x, main.position.y, z);
            if (cam == null) return;
            float single = cam.orthographicSize;
            cam.orthographicSize = single - zoom * single / 10f * 1;
            if (cam.orthographicSize < 5f)
            {
                cam.orthographicSize = 5f;
            }
            if (cam.orthographicSize > 1.5f * GameSettings.Instance.SimulationSize.val)
            {
                cam.orthographicSize = 1.5f * GameSettings.Instance.SimulationSize.val;
            }
            
            
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
    }
}