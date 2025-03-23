using System;
using System.IO;
using System.Reflection;
using HarmonyLib;
using HighQualityTextures;
using UnityEngine;
using Verse;

[HarmonyPatch(typeof(ModMetaData), "Icon", MethodType.Getter)]
public static class IconPatch
{
    static Texture2D customModIcon = null;
    // Prefix-Methode (wird vor dem Originalcode ausgeführt)
    public static bool Prefix(ModMetaData __instance, ref Texture2D __result)
    {
        Type modType = __instance.GetType();

        // Get the FieldInfo für das private Feld "rootDirInt"
        FieldInfo rootDirFieldInfo = modType.GetField("rootDirInt", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo iconImageWasLoadedFieldInfo = modType.GetField("iconImageWasLoaded", BindingFlags.NonPublic | BindingFlags.Instance);

        // Check if the iconImageWasLoaded field exists and retrieve its value
        if (iconImageWasLoadedFieldInfo != null)
        {
            bool iconImageWasLoaded = (bool)iconImageWasLoadedFieldInfo.GetValue(__instance);
            if (iconImageWasLoaded)
            {
                __result = customModIcon;
                return true; // Führe den Originalcode aus, wenn das Feld bereits auf true gesetzt wurde
            }
        }

        // Sicherstellen, dass das Feld existiert und seinen Wert abrufen
        if (rootDirFieldInfo != null)
        {
            DirectoryInfo rootDirField = rootDirFieldInfo.GetValue(__instance) as DirectoryInfo;

            if (rootDirField != null)
            {
                string ModIconDdsImagePath = Path.Combine(rootDirField.FullName, "About", "ModIcon.dds");

                if (File.Exists(ModIconDdsImagePath))
                {
                    customModIcon = DdsLoader.Load(ModIconDdsImagePath);
                    if (customModIcon != null)
                    {
                        customModIcon.name = "ModIcon";
                        customModIcon.filterMode = FilterMode.Trilinear;
                        customModIcon.Apply(true, true);
                    }
                    else
                    {
                        Debug.LogError($"Failed to load custom icon: {DdsLoader.error}");
                    }

                    // Setze das Ergebnis des Getters
                    __result = customModIcon;

                    // Markiere das Icon als geladen
                    if (iconImageWasLoadedFieldInfo != null)
                    {
                        iconImageWasLoadedFieldInfo.SetValue(__instance, true);
                    }
                    else
                    {
                        Debug.LogError("Field 'iconImageWasLoaded' not found in ModMetaData.");
                    }

                    Debug.Log($"Custom Icon {ModIconDdsImagePath} loaded.");
                    return false; // Überspringe den Originalcode
                }
            }
            else
            {
                Debug.LogError("rootDirField is null. Ensure the field is properly initialized.");
            }
        }
        else
        {
            Debug.LogError("Field 'rootDirInt' not found in ModMetaData.");
        }

        return true; // Führe den Originalcode aus, wenn kein benutzerdefiniertes Icon geladen wurde
    }
}