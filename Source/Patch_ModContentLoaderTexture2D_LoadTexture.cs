using HarmonyLib;
using RimWorld.IO;
using System.IO;
using UnityEngine;
using Verse;
using Log = HighQualityTextures.Utils.Log;

namespace HighQualityTextures
{
    [HarmonyPatch(typeof(ModContentLoader<Texture2D>), "LoadTexture")]
    class Patch_ModContentLoaderTexture2D_LoadTexture
    {
        static bool Prefix(VirtualFile file, ref Texture2D __result)
        {
            Texture2D texture2D = null;

            string filePath = file.FullPath;

            if (filePath.EndsWith(".dds", System.StringComparison.OrdinalIgnoreCase))
            {
                texture2D = DdsLoader.Load(filePath);
                if (texture2D != null)
                {
                    texture2D.name = Path.GetFileNameWithoutExtension(filePath);
                    texture2D.filterMode = FilterMode.Trilinear;
                    texture2D.Apply(true, true);
                }
                else
                {
                    Log.Error($"Failed to load DDS texture: {DdsLoader.error}");
                }
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
                // Log.Message("Texture loaded successfully");
                return false;
            }

            Log.Warning("Texture loading failed");
            return true;
        }
    }
}