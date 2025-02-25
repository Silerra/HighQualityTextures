using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Verse;
using Log = HighQualityTextures.Utils.Log;

namespace HighQualityTextures
{
    [HarmonyPatch(typeof(ModAssetBundlesHandler))]
    static class Patch_ModAssetBundlesHandler
    {
        [HarmonyPatch(MethodType.Constructor, new Type[] { typeof(ModContentPack) })]
        public static void Prefix(ModAssetBundlesHandler __instance)
        {
            FieldInfo field = AccessTools.Field(typeof(ModAssetBundlesHandler), "TextureExtensions");
            if (field == null)
            {
                Log.Error("Field TextureExtensions not found!");
                return;
            }

            string[] currentExtensions = (string[])field.GetValue(null);
            if (Array.Exists(currentExtensions, ext => ext.Equals(".dds", StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            string[] newExtensions = new string[currentExtensions.Length + 1];
            Array.Copy(currentExtensions, newExtensions, currentExtensions.Length);
            newExtensions[newExtensions.Length - 1] = ".dds";

            field.SetValue(null, newExtensions);
            Log.Message("Added '.dds' to TextureExtensions");
            Log.Message($"TextureExtensions: {string.Join(", ", newExtensions)}");
        }
    }
}