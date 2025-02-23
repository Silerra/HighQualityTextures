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
            Log.Message("HighQualityTextures: Initializing Harmony patches");
            var harmony = new Harmony("de.silerra.highqualitytextures");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
