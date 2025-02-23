using System.Reflection;

namespace HighQualityTextures.Utils
{
    public class Log
    {
        private static readonly string ModNameSpace = Assembly.GetExecutingAssembly().GetName().Name;

        public static void Message(string message)
        {
            Verse.Log.Message($"[{ModNameSpace}]: {message}");
        }

        public static void Warning(string message)
        {
            Verse.Log.Warning($"[{ModNameSpace}]: {message}");
        }

        public static void Error(string message)
        {
            Verse.Log.Error($"[{ModNameSpace}]: {message}");
        }
    }
}