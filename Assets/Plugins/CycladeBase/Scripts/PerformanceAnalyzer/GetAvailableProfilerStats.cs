using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.Profiling;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;

namespace CycladeBase.PerformanceAnalyzer
{
    public class GetAvailableProfilerStats
    {
        /***********************************************************************************************************************************************
    * Source: https://docs.unity3d.com/2020.2/Documentation/ScriptReference/Unity.Profiling.LowLevel.Unsafe.ProfilerRecorderHandle.GetAvailable.html
    ************************************************************************************************************************************************/
        struct StatInfo
        {
            public ProfilerCategory Cat;
            public string Name;
            public ProfilerMarkerDataUnit Unit;
        }

        public static void Run()
        {
            var availableStatHandles = new List<ProfilerRecorderHandle>();
            ProfilerRecorderHandle.GetAvailable(availableStatHandles);

            var availableStats = new List<StatInfo>(availableStatHandles.Count);
            foreach (var h in availableStatHandles)
            {
                var statDesc = ProfilerRecorderHandle.GetDescription(h);
                var statInfo = new StatInfo()
                {
                    Cat = statDesc.Category,
                    Name = statDesc.Name,
                    Unit = statDesc.UnitType
                };
                availableStats.Add(statInfo);
            }
            availableStats.Sort((a, b) =>
            {
                var result = string.Compare(a.Cat.ToString(), b.Cat.ToString());
                if (result != 0)
                    return result;

                return string.Compare(a.Name, b.Name);
            });

            var sb = new StringBuilder("Available stats:\n");
            foreach (var s in availableStats)
            {
                sb.AppendLine($"{s.Cat.ToString()}\t\t - {s.Name}\t\t - {s.Unit}");
            }

            WriteToFile("AvailableStats.txt", sb.ToString());
        }
        
        public static bool WriteToFile(string a_FileName, string a_FileContents)
        {
            var fullPath = Path.Combine(Application.persistentDataPath, a_FileName);
            try
            {
                File.WriteAllText(fullPath, a_FileContents);
                Debug.Log($"AvailableStats here: {fullPath}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to write to {fullPath} with exception {e}");
                return false;
            }
        }
    }
}