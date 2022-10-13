using ManagementScripts;
using SettingScripts;
using SimulationScripts;
using SimulationScripts.BibiteScripts;
using UnityEngine;

namespace TwitchIntegration
{
    public static class Commands
    {
        public static string? ReloadSettings()
        {
            Main.loggerInstance?.Msg("Reloading");
            Settings.Load();
            Main.loggerInstance?.Msg("Reloaded");
            return "Reloaded settings";
        }


        public static string? layAll(int amount = 1)
        {
            int x = 0;
            GameObject[] array3 = GameObject.FindGameObjectsWithTag("bibite");
            foreach (GameObject t in array3)
            {
                if (t.TryGetComponent(out BibiteControl bc))
                {
                    for (int y = 0; y < amount; y++)
                    {
                        x++;
                        bc.LayEgg();
                    }
                }
            }
            return $"Laid {x} eggs"; 
        }

        public static string? lay(GameObject bibite, int amount = 1)
        {
            if (bibite.TryGetComponent(out BibiteControl bc))
            {
                for (int y = 0; y < amount; y++)
                {
                    bc.LayEgg();
                }
                return $"Laid {amount} eggs";
            }
            return "Failed to lay eggs";
        }

        public static string? pushAll(int factor = 10, float min = -1f, float max = 1f)
        {
            int x = 0;
            GameObject[] array3 = GameObject.FindGameObjectsWithTag("bibite");
            foreach (GameObject t in array3)
            {
                if (t.TryGetComponent(out Rigidbody2D rb))
                {
                    x++;
                    Vector2 randomVector = new Vector2(Random.Range(min, max), Random.Range(min, max));
                    rb.velocity = (factor * 4000 * randomVector / Time.timeScale);
                }
            }
            return $"Pushed {x} Bibites";
        }

        public static string? push(GameObject entity, int factor = 10, float min = -1f, float max = 1f)
        {
            if (entity.TryGetComponent(out Rigidbody2D rb))
            {
                Vector2 randomVector = new Vector2(Random.Range(min, max), Random.Range(min, max));
                rb.velocity = (factor * 4000 * randomVector / Time.timeScale);
                return "Pushed current Bibite";
            }
            return "Failed to push Bibite";
        }

        public static string? UpdateBibiteCap(int max)
        {
            BibiteSpawner.UpdateBibiteCap(max != 0);
            BibiteSpawner.UpdateCapNumber(max);
            string res = (max == 0 ? "Bibite cap is now disabled" : $"Bibite cap is now set to {max}");
            return res;
        }

        public static string? UpdateBibiteLimit(int max)
        {
            BibiteSpawner.UpdateBibiteLimit(max != 0);
            BibiteSpawner.UpdateLimitNumber(max);
            string res = (max == 0 ? "Bibite limit is now disabled" : $"Bibite limit is now set to {max}");
            return res;
        }

        public static string? SetSpeed(float speed)
        {
            TimeController.Instance.timeSlider.value = Tools.FactorToSlider(speed);
            return $"Simulation speed is now set to {Tools.SliderToFactor(TimeController.Instance.timeSlider.value)}";
        }

        public static string? SelectRandomEntity()
        {
            GameObject[] gameObjectsWithTag = GameObject.FindGameObjectsWithTag("bibite");
            if (gameObjectsWithTag.Length != 0)
            {
                UserControl.Instance.SelectTarget(gameObjectsWithTag[Random.Range(0, gameObjectsWithTag.Length)]);

            }

            return null;
        }

        public static string? ZoomIn()
        {
            return Zoom(1);
        }

        public static string? ZoomOut()
        {
            return Zoom(-1);
        }

        public static string? Zoom(float zoom)
        {
            var cam = Camera.main;
            // float z = Tools.MinMaxDefault(main.position.z - zoom, 1, 100);
            // if (main != null) main.position = new Vector3(main.position.x, main.position.y, z);
            if (cam == null) return "No camera found";
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

            return null;
        }

        public static string? SetTag(string s)
        {
            var a = Tools.GetClosestEntity();
            if (a != null && a.TryGetComponent(out BibiteGenes bibiteGenes))
            {
                bibiteGenes.speciesTag = s;
                return s.Length == 0 ? "Removed tag" : $"Set tag to {s}";
            }
            return "Failed to set tag";
        }
    }
}