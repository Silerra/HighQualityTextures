using System;
using System.IO;
using System.Reflection;
using HarmonyLib;
using HighQualityTextures;
using UnityEngine;
using Verse;
using Log = HighQualityTextures.Utils.Log;

[HarmonyPatch(typeof(ModMetaData), "Icon", MethodType.Getter)]
public static class IconPatch
{
    private static void SetcustomModIcon(ModMetaData instance, Texture2D value)
    {
        FieldInfo iconImageField = typeof(ModMetaData).GetField("iconImage", BindingFlags.NonPublic | BindingFlags.Instance);
        if (iconImageField == null)
        {
            Log.Error("Field 'iconImage' not found in ModMetaData.");
            return;
        }

        iconImageField.SetValue(instance, value);
    }

    private static Texture2D GetCustomModIcon(ModMetaData instance)
    {
        FieldInfo iconImageField = typeof(ModMetaData).GetField("iconImage", BindingFlags.NonPublic | BindingFlags.Instance);
        if (iconImageField == null)
        {
            Log.Error("Field 'iconImage' not found in ModMetaData.");
            return null;
        }

        return iconImageField.GetValue(instance) as Texture2D;
    }

    // Prefix-Methode (wird vor dem Originalcode ausgeführt)
    public static bool Prefix(ModMetaData __instance, ref Texture2D __result)
    {
        Texture2D CustomModIcon(ModMetaData instance) => GetCustomModIcon(instance);
        if (IsIconAlreadyLoaded(__instance))
        {
            __result = CustomModIcon(__instance);
            return true; // Execute original code
        }
        if (!__instance.ModIconPath.NullOrEmpty())
        {
            return true; // Execute original code
        }

        DirectoryInfo rootDir = GetRootDir(__instance);
        if (rootDir == null)
        {
            Log.Warning("Root directory is null. Ensure the field 'rootDirInt' is properly initialized.");
            return true; // Execute original code
        }

        string modIconPath = Path.Combine(rootDir.FullName, "About", "ModIcon.dds");
        if (!File.Exists(modIconPath))
        {
            return true; // Execute original code if no custom icon file exists
        }

        // Set the custom mod icon
        SetcustomModIcon(__instance, LoadCustomIcon(modIconPath));
        if (CustomModIcon(__instance) == null)
        {
            Log.Error($"Failed to load custom icon from {modIconPath}");
            return true; // Execute original code
        }

        __result = CustomModIcon(__instance);
        MarkIconAsLoaded(__instance);
        Log.Message($"Custom Icon {modIconPath} loaded successfully.");
        return false; // Skip original code
    }

    // Prüft, ob das Icon bereits geladen wurde
    private static bool IsIconAlreadyLoaded(ModMetaData instance)
    {
        FieldInfo iconImageWasLoadedField = instance.GetType().GetField("iconImageWasLoaded", BindingFlags.NonPublic | BindingFlags.Instance);
        if (iconImageWasLoadedField == null)
        {
            Log.Warning("Field 'iconImageWasLoaded' not found in ModMetaData.");
            return false;
        }
        // Log.Log($"Icon was already loaded: {iconImageWasLoadedField.GetValue(instance)}");

        return (bool)iconImageWasLoadedField.GetValue(instance);
    }

    // Ruft das Root-Verzeichnis des Mods ab
    private static DirectoryInfo GetRootDir(ModMetaData instance)
    {
        FieldInfo rootDirField = instance.GetType().GetField("rootDirInt", BindingFlags.NonPublic | BindingFlags.Instance);
        if (rootDirField == null)
        {
            Log.Error("Field 'rootDirInt' not found in ModMetaData.");
            return null;
        }

        return rootDirField.GetValue(instance) as DirectoryInfo;
    }

    // Lädt das benutzerdefinierte Icon
    private static Texture2D LoadCustomIcon(string path)
    {
        Texture2D texture = DdsLoader.Load(path);
        if (texture != null)
        {
            texture.name = "ModIcon";
            texture.filterMode = FilterMode.Trilinear;
            texture.Apply(true, true);
        }
        return texture;
    }

    // Markiert das Icon als geladen
    private static void MarkIconAsLoaded(ModMetaData instance)
    {
        FieldInfo iconImageWasLoadedField = instance.GetType().GetField("iconImageWasLoaded", BindingFlags.NonPublic | BindingFlags.Instance);
        if (iconImageWasLoadedField != null)
        {
            iconImageWasLoadedField.SetValue(instance, true);
        }
        else
        {
            Log.Warning("Field 'iconImageWasLoaded' not found in ModMetaData.");
        }
    }
}