using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace Phrasefable.StardewMods.AggressiveAcorns
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Required by harmony.")]
    internal class TreePatches
    {
        public static void IsPassable_Postfix(Tree __instance, ref bool __result)
        {
            try
            {
                __result = __instance.health.Value <= -99 ||
                           __instance.growthStage.Value <= AggressiveAcorns.Config.MaxPassableGrowthStage;
            }
            catch (Exception ex)
            {
                AggressiveAcorns.ErrorLogger($"Failed in {nameof(TreePatches.IsPassable_Postfix)}:\n{ex}");
            }
        }


        public static bool PerformToolAction_Prefix(Tool t, ref bool __result)
        {
            try
            {
                if (AggressiveAcorns.Config.ProtectFromMelee && t is MeleeWeapon)
                {
                    __result = false; // Tool action does nothing
                    return false;     // Prevent further processing (other prefixes or original method)
                }

                return true; // Don't care, so normal processing
            }
            catch (Exception ex)
            {
                AggressiveAcorns.ErrorLogger(
                    $"Harmony Patch failed in {nameof(TreePatches.PerformToolAction_Prefix)}:\n{ex}"
                );
                return true; // Allow original method (& other patches) to run.
            }
        }


        public static bool DayUpdate_Prefix(
            Tree __instance,
            GameLocation environment,
            Vector2 tileLocation,
            NetBool ___destroy)
        {
            try
            {
                // TODO check if there is any need to do the skip-update-of-first-day-when-spread thing
                bool isDestroyed = __instance.DestroyIfDead(___destroy);

                __instance.ValidateTapped(environment, tileLocation);

                if (!isDestroyed && environment.TreeCanGrowAt(__instance, tileLocation))
                {
                    __instance.PopulateSeed();
                    __instance.TrySpread(environment, tileLocation);
                    __instance.TryIncreaseStage(environment, tileLocation);
                    __instance.ManageHibernation(environment, tileLocation);
                    __instance.TryRegrow(environment, tileLocation);
                }

                return false; // Prevent other processing on the method.
            }
            catch (Exception ex)
            {
                AggressiveAcorns.ErrorLogger($"Harmony Patch failed in {nameof(TreePatches.DayUpdate_Prefix)}:\n{ex}");
                return true; // Allow original method (& other patches) to run.
            }
        }
    }
}
