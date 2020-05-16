using System;
using System.Reflection;
using Common.Harmony;
using Common.Harmony.PatchValidation;
using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace AggressiveAcorns
{
    public static class Patch_Tree_PerformToolAction
    {
        private static IMonitor _logger;

        private static readonly MethodInfo PatchTarget = AccessTools.Method(
            typeof(Tree),
            nameof(Tree.performToolAction),
            new[] {typeof(Tool), typeof(int), typeof(Vector2), typeof(GameLocation)}
        );

        private static readonly MethodInfo PatchSource = AccessTools.Method(
            typeof(Patch_Tree_PerformToolAction),
            nameof(Patch_Tree_PerformToolAction.PerformToolAction_Prefix)
        );


        internal static HarmonyPatchInfo Initialize(IMonitor monitor)
        {
            _logger = monitor;
            return new HarmonyPatchInfo(PatchTarget, PatchSource, PrefixPatch.Instance, ExclusivePatch.Instance);
        }


        private static bool PerformToolAction_Prefix(Tool t, ref bool __result)
        {
            try
            {
                if (t is MeleeWeapon)
                {
                    __result = false; // Tool action does nothing
                    return false;     // Prevent further processing (other prefixes or original method)
                }

                return true; // Don't care, so normal processing
            }
            catch (Exception ex)
            {
                _logger.Log($"Harmony Patch failed in {nameof(PerformToolAction_Prefix)}:\n{ex}", LogLevel.Error);
                // Allow original method (& other patches) to run.
                return true;
            }
        }
    }
}
