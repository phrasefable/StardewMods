using System;
using System.Reflection;
using Common.Harmony;
using Common.Harmony.PatchTypes;
using Common.Harmony.PatchValidation;
using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace AggressiveAcorns
{
    public static class Patch_Tree_IsPassible
    {
        private static int _maxPassibleGrowthStage;
        private static IMonitor _logger;

        private static readonly MethodInfo PatchTarget = AccessTools.Method(
            typeof(Tree),
            nameof(Tree.isPassable),
            new[] {typeof(Character)}
        );

        private static readonly MethodInfo PatchSource = AccessTools.Method(
            typeof(Patch_Tree_IsPassible),
            nameof(Patch_Tree_IsPassible.IsPassible_Postfix)
        );


        internal static HarmonyPatchInfo Initialize(IMonitor monitor, IModConfig config)
        {
            _logger = monitor;
            _maxPassibleGrowthStage = config.MaxPassibleGrowthStage;

            return new HarmonyPatchInfo(PatchTarget, PatchSource, PostfixPatch.Instance, ExclusivePatch.Instance);
        }


        // ReSharper disable once RedundantAssignment
        private static void IsPassible_Postfix(Tree __instance, ref bool __result)
        {
            try
            {
                __result = __instance.health.Value <= -99 || __instance.growthStage.Value <= _maxPassibleGrowthStage;
            }
            catch (Exception ex)
            {
                _logger.Log($"Failed in {nameof(IsPassible_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }
    }
}
