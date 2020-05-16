using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Dummy1
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            harmony.Patch(
                typeof(Tree).GetMethod(nameof(Tree.dayUpdate)),
                new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.TreeDayUpdate))
            );
            harmony.Patch(
                typeof(Tree).GetMethod(nameof(Tree.isPassable)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.IsPassible_Postfix))
            );
        }

        public static bool TreeDayUpdate(
            Tree __instance,
            GameLocation environment,
            Vector2 tileLocation,
            NetBool ___destroy)
        {
            return true;
        }

        // ReSharper disable once RedundantAssignment
        private static void IsPassible_Postfix(Tree __instance, ref bool __result) { }
    }
}
