using System;
using System.Collections.Generic;
using ManagementScripts;
using SimulationScripts.BibiteScripts;
using UnityEngine;

namespace TwitchIntegration
{
    public static class Tools
    {
        public static float SliderToFactor(int sliderValue)
        {
            return Mathf.Pow(5f, sliderValue);
        }

        public static float FactorToSlider(float factor)
        {
            return Mathf.Log(factor, 5f);
        }

        public static T MinMaxDefault<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
                return min;
            if (value.CompareTo(max) > 0)
                return max;
            return value;
        }


        public static string Slice(this string str, int start, int end)
        {
            if (end < 0)
            {
                end = str.Length + end;
            }

            int len = end - start;
            return str.Substring(start, len);
        }

        public static string Slice(this string str, int start)
        {
            return str.Slice(start, str.Length);
        }


        public static List<T> Splice<T>(this List<T> source, int index, int count)
        {
            Main.loggerInstance?.Msg("Splicing list");
            var items = source.GetRange(index, count);
            source.RemoveRange(index, count);
            return items;
        }


        public static GameObject? GetClosestEntity(bool Bibites = true, bool Eggs = false, GameObject? notThis = null)
        {
            float num = float.MaxValue;
            if (Camera.main == null)
            {
                Main.loggerInstance?.Msg("There is not camera... WHAT?");
                return null;
            }

            Vector3 position = Camera.main.transform.position;
            GameObject? newTarget = (GameObject)null;
            GameObject? _ = (GameObject)null;
            if (Bibites)
                foreach (Transform transform in WorldObjectsSpawner.Instance.bibiteHolder.transform)
                {
                    float sqrMagnitude = (transform.position - position).sqrMagnitude;
                    if ((double)sqrMagnitude < (double)num)
                    {
                        num = sqrMagnitude;
                        _ = transform.gameObject;

                        if (_ == notThis)
                            continue;

                        newTarget = _;
                    }
                }

            if (Eggs)
                foreach (Transform transform in WorldObjectsSpawner.Instance.colorKillerHolder.transform)
                {
                    float sqrMagnitude = (transform.position - position).sqrMagnitude;
                    if ((double)sqrMagnitude < (double)num)
                    {
                        num = sqrMagnitude;
                        _ = transform.gameObject;
                        if (_ == notThis)
                            continue;
                        newTarget = _;
                    }
                }

            return newTarget;
        }

        public static int GetHighestGeneration()
        {
            try
            {
                GameObject[] array3 = GameObject.FindGameObjectsWithTag("bibite");
                int num = 0;
                foreach (GameObject t in array3)
                {
                    BibiteGenes? a = t.GetComponent<BibiteGenes>();
                    int num2 = (a == null ? 0 : a.generation);
                    if (num2 > num)
                    {
                        num = num2;
                    }
                }

                if (Settings.Instance.debugMode)
                    Main.loggerInstance?.Msg($"Highest generation: {num}");
                return num;
            }
            catch (Exception e)
            {
            }

            return 0;
        }
    }
}