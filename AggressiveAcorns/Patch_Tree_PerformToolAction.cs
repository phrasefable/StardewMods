using System;
using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace AggressiveAcorns
{
    public class Patch_Tree_PerformToolAction : PrefixPatch
    {
        protected override Type TargetType => typeof(Tree);

        protected override string TargetName => nameof(Tree.performToolAction);

        protected override Type[] TargetParameters => new[]
        {
            typeof(Tool),
            typeof(int),
            typeof(Vector2),
            typeof(GameLocation)
        };

        protected override string PatchMethod => nameof(Prefix);

        public override bool IsValid(HarmonyInstance harmony, out string errors)
        {
            return IsExclusivePatch(harmony, out errors);
        }

        private static bool Prefix(Tool t, ref bool __result)
        {
            if (t is MeleeWeapon)
            {
                __result = false;
                return false;
            }

            return true;
        }
    }
}