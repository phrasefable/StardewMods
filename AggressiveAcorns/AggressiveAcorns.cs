using HarmonyLib;
using Phrasefable.StardewMods.Common.Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace Phrasefable.StardewMods.AggressiveAcorns
{
    public class AggressiveAcorns : Mod
    {
        private static Action<string> ErrorLogger;
        internal static ModConfig Config;


        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();
            ErrorLogger = message => this.Monitor.Log(message, LogLevel.Error);

            this.ApplyPatches();
        }


        private void ApplyPatches()
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Postfix(AccessTools.Method(typeof(Tree), nameof(Tree.isPassable)),
                            AccessTools.Method(typeof(AggressiveAcorns), nameof(IsPassable_Postfix)));

            harmony.Prefix(AccessTools.Method(typeof(Tree), nameof(Tree.performToolAction)),
                           AccessTools.Method(typeof(AggressiveAcorns), nameof(PerformToolAction_Prefix)));
        }


        public static void IsPassable_Postfix(Tree __instance, ref bool __result)
        {
            try
            {
                __result = __instance.health.Value <= -99 ||
                           __instance.growthStage.Value <= Config.MaxPassableGrowthStage;
            }
            catch (Exception ex)
            {
                ErrorLogger($"Failed in {nameof(IsPassable_Postfix)}:\n{ex}");
            }
        }


        public static bool PerformToolAction_Prefix(Tool t, ref bool __result, Tree __instance)
        {
            try
            {
                if (!Config.DoMeleeWeaponsDestroySeedlings && t is MeleeWeapon &&
                    (__instance.growthStage.Value == Tree.sproutStage || __instance.growthStage.Value == Tree.saplingStage))
                {
                    __result = false; // Tool action does nothing
                    return false;     // Prevent further processing (other prefixes or original method)
                }

                return true; // Don't care, so normal processing
            }
            catch (Exception ex)
            {
                ErrorLogger(
                    $"Harmony Patch failed in {nameof(PerformToolAction_Prefix)}:\n{ex}"
                );
                return true; // Allow original method (& other patches) to run.
            }
        }
    }
}
