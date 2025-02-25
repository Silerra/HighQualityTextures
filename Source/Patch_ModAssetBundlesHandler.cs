using System;
using System.Reflection;
using HarmonyLib;
using Verse;
using Log = HighQualityTextures.Utils.Log;

namespace HighQualityTextures
{
    [HarmonyPatch(typeof(ModAssetBundlesHandler))]
    [HarmonyPatch("ReloadAll")]
    public static class Patch_ModAssetBundlesHandler_ReloadAll
    {
        public static void Prefix(ModAssetBundlesHandler __instance)
        {
            try
            {
                // Zugriff auf die Klasse ModAssetBundlesHandler
                Type handlerType = typeof(ModAssetBundlesHandler);

                // Hole das statische Feld "TextureExtensions"
                FieldInfo field = handlerType.GetField(
                    "TextureExtensions",
                    BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public
                );

                if (field == null)
                {
                    Log.Error("Field TextureExtensions not found!");
                    return;
                }

                // Aktuelles Array auslesen
                string[] currentExtensions = (string[])field.GetValue(null);
                Log.Message($"Current TextureExtensions: {string.Join(", ", currentExtensions)}");

                // Prüfe, ob ".dds" bereits vorhanden ist
                if (Array.Exists(currentExtensions, ext => ext.Equals(".dds", StringComparison.OrdinalIgnoreCase)))
                {
                    Log.Message(".dds already exists in TextureExtensions");
                    return;
                }

                // Erweitere das Array um ".dds"
                string[] newExtensions = new string[currentExtensions.Length + 1];
                Array.Copy(currentExtensions, newExtensions, currentExtensions.Length);
                newExtensions[newExtensions.Length - 1] = ".dds";

                // Setze das modifizierte Array zurück
                field.SetValue(null, newExtensions);

                Log.Message($"Successfully added .dds to TextureExtensions. New TextureExtensions: {string.Join(", ", newExtensions)}");
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to patch TextureExtensions: {ex}");
            }
        }
    }
}