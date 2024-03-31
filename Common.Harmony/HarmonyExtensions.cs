using System.Reflection;
using HarmonyLib;

namespace Phrasefable.StardewMods.Common.Harmony
{
    internal static class HarmonyExtensions
    {
        public static void Postfix(this HarmonyLib.Harmony harmony, MethodInfo target, MethodInfo source)
        {
            harmony.Patch(target, postfix: new HarmonyMethod(source));
        }
        
        public static void Prefix(this HarmonyLib.Harmony harmony, MethodInfo target, MethodInfo source)
        {
            harmony.Patch(target, prefix: new HarmonyMethod(source));
        }
    }
}