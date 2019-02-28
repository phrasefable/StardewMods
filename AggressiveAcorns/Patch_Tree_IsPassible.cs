using System;
using Harmony;
using PhraseLib;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace AggressiveAcorns
{
    public class Patch_Tree_IsPassible : PrefixPatch
    {
        public static int MaxPassibleGrowthStage;

        public Patch_Tree_IsPassible(int maxPassibleGrowthStage)
        {
            MaxPassibleGrowthStage = maxPassibleGrowthStage;
        }

        public override bool IsValid(HarmonyInstance harmony, out string errors)
        {
            return IsExclusivePatch(harmony, out errors);
        }

        protected override Type TargetType => typeof(Tree);
        protected override string TargetName => nameof(Tree.isPassable);
        protected override Type[] TargetParameters => new[] {typeof(Character)};
        protected override string PatchMethod => nameof(IsPassible);

        private static bool IsPassible(Tree __instance, ref bool __result)
        {
            __result = __instance.health.Value <= -99 || __instance.growthStage.Value <= MaxPassibleGrowthStage;

            return false;
        }
    }
}