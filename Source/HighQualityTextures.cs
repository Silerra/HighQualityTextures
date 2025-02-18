using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Verse;

namespace HighQualityTextures
{
    public class HighQualityTextures :Mod
    {
        public HighQualityTextures(ModContentPack pack) :base(pack) {
            var harmony = new Harmony("de.silerra.highqualitytextures");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }   
    }
}
