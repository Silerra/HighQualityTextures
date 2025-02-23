using HarmonyLib;
using RimWorld.IO;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using Verse;

namespace HighQualityTextures
{

    [HarmonyPatch(typeof(ModContentLoader<Texture2D>), "LoadTexture")]
    class PatchModContentLoaderTexture2D
    {
        static bool Prefix(VirtualFile file, ref Texture2D __result)
        {
            Log.Message("Loading texture: " + file.FullPath);
            Texture2D texture2D = null;

            string filePath = file.FullPath;
            string ddsPath = Path.ChangeExtension(filePath, ".dds");
            if (File.Exists(ddsPath))
            {
                texture2D = DdsLoader.Load(ddsPath);
                texture2D.name = Path.GetFileNameWithoutExtension(filePath);
                texture2D.filterMode = FilterMode.Trilinear;
                texture2D.Apply(true, true);
            }
            else if (File.Exists(filePath))
            {
                byte[] data = file.ReadAllBytes();
                texture2D = new Texture2D(2, 2, TextureFormat.Alpha8, true);
                texture2D.LoadImage(data);
                texture2D.name = Path.GetFileNameWithoutExtension(filePath);
                texture2D.filterMode = FilterMode.Trilinear;
                texture2D.Apply(true, true);
            }

            if (texture2D != null)
            {
                __result = texture2D;
                return false;
            }

            return true;
        }
    }
}
