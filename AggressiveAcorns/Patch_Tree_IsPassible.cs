using System;
using PhraseLib;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace AggressiveAcorns {

    internal class Patch_Tree_IsPassible : HarmonyPatch {
        private static int _maxPassibleGrowthStage;


        public Patch_Tree_IsPassible(int maxPassibleGrowthStage) {
            _maxPassibleGrowthStage = maxPassibleGrowthStage;
        }


        /* public override bool IsValid(HarmonyInstance harmony, out string errors) {
             return IsExclusivePatch(harmony, out errors);
         }*/


        protected /*override*/ Type TargetType => typeof(Tree);
        protected /*override*/ string TargetName => nameof(Tree.isPassable);
        protected /*override*/ Type[] TargetParameters => new[] {typeof(Character)};
        protected /*override*/ string PatchMethod => nameof(IsPassible);


        // ReSharper disable once RedundantAssignment
        private static bool IsPassible(Tree __instance, ref bool __result) {
            __result = __instance.health.Value <= -99 || __instance.growthStage.Value <= _maxPassibleGrowthStage;

            return false;
        }
    }

}