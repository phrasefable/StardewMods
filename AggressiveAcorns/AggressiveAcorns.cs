using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
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


        private Harmony _harmony;
        private ICollection<IHarmonyPatchInfo> _patches;


        public override void Entry([JetBrains.Annotations.NotNull] IModHelper helper)
        {
            AggressiveAcorns.Config = new ConfigAdaptor(helper.ReadConfig<ModConfig>());
            AggressiveAcorns.ErrorLogger = message => this.Monitor.Log(message, LogLevel.Error);

            this.SetUpPatches();

            helper.ConsoleCommands.Add(
                "aa_update_all",
                "Calls DayUpdate on all trees in current location",
                (name, args) =>
                {
                    if (args.Length > 1)
                    {
                        this.Monitor.Log($"Invalid arguments '{args}'");
                        return;
                    }

                    int reps = 1;
                    if (args.Length == 1)
                    {
                        bool isInt = int.TryParse(args[0], out reps);
                        if (!isInt)
                        {
                            this.Monitor.Log($"Not an int '{args}'");
                            return;
                        }
                    }

                    GameLocation location = Game1.player.currentLocation;
                    for (int i = 0; i < reps; i++)
                    {
                        IEnumerable<Tree> trees = location.terrainFeatures.Values
                            .Where(feature => feature is Tree)
                            .Cast<Tree>()
                            .ToList();

                        foreach (Tree tree in trees)
                        {
                            tree.dayUpdate();
                        }
                    }
                }
            );
        }


        private void SetUpPatches()
        {
            this._harmony = new Harmony(this.ModManifest.UniqueID);
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


        public static bool PerformToolAction_Prefix(Tool t, ref bool __result)
        {
            try
            {
                if (!AggressiveAcorns.Config.DoMeleeWeaponsDestroySeedlings && t is MeleeWeapon)
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


        public static bool DayUpdate_Prefix(
            Tree __instance,
            // GameLocation environment,
            // Vector2 tileLocation,
            NetBool ___destroy,
            ref float ___shakeRotation)
        {
            try
            {
                //__instance.DayUpdateAggressively(environment, tileLocation, ___destroy, ref ___shakeRotation);
                __instance.DayUpdateAggressively(___destroy, ref ___shakeRotation);
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
