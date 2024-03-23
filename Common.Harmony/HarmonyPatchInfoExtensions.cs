using System;
using HarmonyLib;

namespace Phrasefable.StardewMods.Common.Harmony
{
    internal static class HarmonyPatchInfoExtensions
    {
        public static void Apply(this IHarmonyPatchInfo patch, HarmonyLib.Harmony harmony)
        {
            switch (patch.PatchType)
            {
                case PatchType.Prefix:
                    harmony.Patch(patch.PatchTarget, prefix: new HarmonyMethod(patch.PatchSource));
                    break;
                case PatchType.Postfix:
                    harmony.Patch(patch.PatchTarget, postfix: new HarmonyMethod(patch.PatchSource));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("patch.PatchType", "Unexpected patch type, must be prefix or postfix");
            }
        }
    }
}
