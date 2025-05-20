using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using GameFramework.Localization;

using HarmonyLib;

namespace ExternalTranslate;


public static class CsvReader
{
    public static List<string[]> ReadCsv(string path)
    {
        var result = new List<string[]>();

        if (!File.Exists(path))
        {
            UnityEngine.Debug.LogWarning($"[CSV] File not found: {path}");
            return result;
        }

        foreach (var line in File.ReadLines(path))
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                string[] values = line.Split(',');
                result.Add(values);
            }
        }

        return result;
    }
}

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
        
    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}

[BepInPlugin("com.example.localizationdump", "Localization Dumper", "1.0.0")]
public class LocalizationDumperPlugin : BaseUnityPlugin
{
    void Awake()
    {
        var harmony = new Harmony("com.example.localizationdump");
        harmony.PatchAll();
    }
}

[HarmonyPatch]
public class XmlLocalizationHelperPatch
{
    static MethodBase TargetMethod() =>
        AccessTools.Method("XmlLocalizationHelper:ParseData");

    static void Postfix(ILocalizationManager localizationManager)
    {
        if (localizationManager != null)
        {
            FileLog.Log("[Harmony] LocalizationManager content:");
            string csvPath = "test.csv";
            FileLog.Log($"[Harmony] Reading CSV path: {csvPath}");
            var rows = CsvReader.ReadCsv(csvPath);

            foreach (var row in rows)
            {
                FileLog.Log($"CSV row: {string.Join(" | ", row)}");
            }
        }
        else
        {
            FileLog.Log("[Harmony] LocalizationManager is null.");
        }
        
    }
}