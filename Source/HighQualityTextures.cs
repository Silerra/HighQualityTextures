using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;
using Log = HighQualityTextures.Utils.Log;

namespace HighQualityTextures
{
    [StaticConstructorOnStartup]
    public class HighQualityTextures : Mod
    {
        public HighQualityTextures(ModContentPack pack) : base(pack)
        {
            Log.Message("Initializing Harmony patches");
            var harmony = new Harmony("de.silerra.highqualitytextures");
            // Füge ".dds" zu AcceptableExtensionsTexture hinzu
            Patch_ModContentLoaderTexture2D.PatchTextureExtensions();
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
