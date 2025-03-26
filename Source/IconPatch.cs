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
    public static Texture2D CustomModIcon { get => GetCustomModIcon(); set => SetcustomModIcon(value); }

    private static void SetcustomModIcon(Texture2D value)
    {
        FieldInfo iconImageField = typeof(ModMetaData).GetField("iconImage", BindingFlags.NonPublic | BindingFlags.Instance);
        if (iconImageField == null)
        {
            Debug.LogError("Field 'iconImage' not found in ModMetaData.");
            return;
        } else
        {
            iconImageField.SetValue(null, value);
        }
    }

    private static Texture2D GetCustomModIcon()
    {
        FieldInfo iconImageField = typeof(ModMetaData).GetField("iconImage", BindingFlags.NonPublic | BindingFlags.Instance);
        if (iconImageField == null)
        {
            Debug.LogError("Field 'iconImage' not found in ModMetaData.");
            return null;
        }
        return iconImageField.GetValue(null) as Texture2D;
    }

    // Prefix-Methode (wird vor dem Originalcode ausgeführt)
    public static bool Prefix(ModMetaData __instance, ref Texture2D __result)
    {
        if (IsIconAlreadyLoaded(__instance))
        {
            __result = CustomModIcon;
            return true; // Originalcode ausführen
        }
        if (!__instance.ModIconPath.NullOrEmpty())
        {
            return true; // Originalcode ausführen, wenn kein benutzerdefiniertes Icon-Pfad gesetzt ist
        }

        DirectoryInfo rootDir = GetRootDir(__instance);
        if (rootDir == null)
        {
            Debug.LogError("Root directory is null. Ensure the field 'rootDirInt' is properly initialized.");
            return true; // Originalcode ausführen
        }

        string modIconPath = Path.Combine(rootDir.FullName, "About", "ModIcon.dds");
        if (!File.Exists(modIconPath))
        {
            return true; // Originalcode ausführen, wenn keine benutzerdefinierte Icon-Datei vorhanden ist
        }

        CustomModIcon = LoadCustomIcon(modIconPath);
        if (CustomModIcon == null)
        {
            Debug.LogError($"Failed to load custom icon from {modIconPath}");
            return true; // Originalcode ausführen
        }

        __result = CustomModIcon;
        MarkIconAsLoaded(__instance);
        Debug.Log($"Custom Icon {modIconPath} loaded successfully.");
        return false; // Originalcode überspringen
    }

    // Prüft, ob das Icon bereits geladen wurde
    private static bool IsIconAlreadyLoaded(ModMetaData instance)
    {
        FieldInfo iconImageWasLoadedField = instance.GetType().GetField("iconImageWasLoaded", BindingFlags.NonPublic | BindingFlags.Instance);
        if (iconImageWasLoadedField == null)
        {
            Debug.LogError("Field 'iconImageWasLoaded' not found in ModMetaData.");
            return false;
        }
        // Debug.Log($"Icon was already loaded: {iconImageWasLoadedField.GetValue(instance)}");

        return (bool)iconImageWasLoadedField.GetValue(instance);
    }

    private static bool IsModIconPathSet(ModMetaData instance)
    {
        FieldInfo modIconPathField = instance.GetType().GetField("ModIconPath");
        if (modIconPathField == null)
        {
            Debug.LogError("Field 'modIconPath' not found in ModMetaData.");
            return false;
        }
        // Debug.Log($"ModIconPath: {modIconPathField.GetValue(instance)}");

        return !string.IsNullOrEmpty((string)modIconPathField.GetValue(instance));
    }

    // Ruft das Root-Verzeichnis des Mods ab
    private static DirectoryInfo GetRootDir(ModMetaData instance)
    {
        FieldInfo rootDirField = instance.GetType().GetField("rootDirInt", BindingFlags.NonPublic | BindingFlags.Instance);
        if (rootDirField == null)
        {
            Debug.LogError("Field 'rootDirInt' not found in ModMetaData.");
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
            Debug.LogError("Field 'iconImageWasLoaded' not found in ModMetaData.");
        }
    }
}