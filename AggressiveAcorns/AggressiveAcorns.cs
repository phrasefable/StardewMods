using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Harmony;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Netcode;
using Phrasefable.StardewMods.AggressiveAcorns.Config;
using Phrasefable.StardewMods.AggressiveAcorns.Framework;
using Phrasefable.StardewMods.Common.Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace Phrasefable.StardewMods.AggressiveAcorns
{
    [UsedImplicitly]
    public class AggressiveAcorns : Mod
    {
        private static Action<string> ErrorLogger;
        internal static IConfigAdaptor Config;


        private HarmonyInstance _harmony;
        private ICollection<IHarmonyPatchInfo> _patches;


        public override void Entry([NotNull] IModHelper helper)
        {
            AggressiveAcorns.Config = new ConfigAdaptor(helper.ReadConfig<ModConfig>());
            AggressiveAcorns.ErrorLogger = message => this.Monitor.Log(message, LogLevel.Error);

            this.SetUpPatches();
        }


        private void SetUpPatches()
        {
            this._harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            this._patches = new List<IHarmonyPatchInfo>
            {
                new HarmonyPatchInfo(
                    AccessTools.Method(typeof(StardewValley.TerrainFeatures.Tree), nameof(Tree.isPassable)),
                    AccessTools.Method(typeof(AggressiveAcorns), nameof(AggressiveAcorns.IsPassable_Postfix)),
                    PatchType.Postfix
                ),
                new HarmonyPatchInfo(
                    AccessTools.Method(typeof(StardewValley.TerrainFeatures.Tree), nameof(Tree.performToolAction)),
                    AccessTools.Method(typeof(AggressiveAcorns), nameof(AggressiveAcorns.PerformToolAction_Prefix)),
                    PatchType.Prefix
                ),
                new HarmonyPatchInfo(
                    AccessTools.Method(typeof(StardewValley.TerrainFeatures.Tree), nameof(Tree.dayUpdate)),
                    AccessTools.Method(typeof(AggressiveAcorns), nameof(AggressiveAcorns.DayUpdate_Prefix)),
                    PatchType.Prefix
                )
            };

            // TODO - reimplement the non-exclusive patch thing.

            foreach (IHarmonyPatchInfo patch in this._patches)
            {
                patch.Apply(this._harmony);
            }
        }


        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Required by harmony.")]
        public static void IsPassable_Postfix(Tree __instance, ref bool __result)
        {
            try
            {
                __result = __instance.health.Value <= -99 ||
                           __instance.growthStage.Value <= AggressiveAcorns.Config.MaxPassableGrowthStage;
            }
            catch (Exception ex)
            {
                AggressiveAcorns.ErrorLogger($"Failed in {nameof(AggressiveAcorns.IsPassable_Postfix)}:\n{ex}");
            }
        }


        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Required by harmony.")]
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
                    $"Harmony Patch failed in {nameof(AggressiveAcorns.PerformToolAction_Prefix)}:\n{ex}"
                );
                return true; // Allow original method (& other patches) to run.
            }
        }


        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Required by harmony.")]
        public static bool DayUpdate_Prefix(
            Tree __instance,
            GameLocation environment,
            Vector2 tileLocation,
            NetBool ___destroy)
        {
            try
            {
                __instance.DayUpdateAggressively(environment, tileLocation, ___destroy);
                return false; // Prevent other processing on the method.
            }
            catch (Exception ex)
            {
                AggressiveAcorns.ErrorLogger(
                    $"Harmony Patch failed in {nameof(AggressiveAcorns.DayUpdate_Prefix)}:\n{ex}"
                );
                return true; // Allow original method (& other patches) to run.
            }
        }
    }
}
