using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Verse;
using Log = HighQualityTextures.Utils.Log;

namespace HighQualityTextures
{
    [HarmonyPatch(typeof(ModAssetBundlesHandler))]
    [HarmonyPatch(MethodType.Constructor)]
    public static class Patch_ModAssetBundlesHandler
    {
        public static void Postfix()
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

                // Prüfe, ob ".dds" bereits vorhanden ist
                if (Array.Exists(currentExtensions, ext => ext.Equals(".dds", StringComparison.OrdinalIgnoreCase)))
                {
                    return;
                }

                // Erweitere das Array um ".dds"
                string[] newExtensions = new string[currentExtensions.Length + 1];
                Array.Copy(currentExtensions, newExtensions, currentExtensions.Length);
                newExtensions[newExtensions.Length - 1] = ".dds";

                // Setze das modifizierte Array zurück
                field.SetValue(null, newExtensions);

                Log.Message("Successfully added .dds to TextureExtensions.");
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to patch TextureExtensions: {ex}");
            }
        }
    }
}