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
            var currentExtensions = (List<string>)field.GetValue(__instance);
            if (!currentExtensions.Contains(".dds"))
            {
                currentExtensions.Add(".dds");
                field.SetValue(__instance, currentExtensions);
                Log.Message("Added '.dds' to TextureExtensions");
                Log.Message($"TextureExtensions: {string.Join(", ", currentExtensions)}");
            }
        }
    }
}