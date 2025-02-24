using System;
using System.Reflection;
using UnityEngine;
using Verse;
using Log = HighQualityTextures.Utils.Log;

namespace HighQualityTextures
{
    public class Patch_ModContentLoaderTexture2D
    {
        public static void PatchTextureExtensions()
        {
            try
            {
                // Zugriff auf die generische Klasse ModContentLoader<Texture2D>
                Type loaderType = typeof(ModContentLoader<Texture2D>);

                // Hole das statische Feld "AcceptableExtensionsTexture"
                FieldInfo field = loaderType.GetField(
                    "AcceptableExtensionsTexture",
                    BindingFlags.Static | BindingFlags.NonPublic
                );

                if (field == null)
                {
                    Log.Error("[YourMod] Field AcceptableExtensionsTexture not found!");
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
                newExtensions[newExtensions.Length - 1] = ".dds"; // C# 8.0 Index-Operator für letztes Element

                // Setze das modifizierte Array zurück
                field.SetValue(null, newExtensions);

                Log.Message("Successfully added .dds to texture extensions.");
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to patch texture extensions: {ex}");
            }
        }
    }
}