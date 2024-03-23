using HarmonyLib;
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
    public class AggressiveAcorns : Mod
    {
        private static Action<string> ErrorLogger;
        internal static IConfigAdaptor Config;


        private Harmony _harmony;
        private ICollection<IHarmonyPatchInfo> _patches;


        public override void Entry(IModHelper helper)
        {
            Config = new ConfigAdaptor(helper.ReadConfig<ModConfig>());
            ErrorLogger = message => this.Monitor.Log(message, LogLevel.Error);

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
                    AccessTools.Method(typeof(Tree), nameof(Tree.isPassable)),
                    AccessTools.Method(typeof(AggressiveAcorns), nameof(IsPassable_Postfix)),
                    PatchType.Postfix
                ),
                new HarmonyPatchInfo(
                    AccessTools.Method(typeof(Tree), nameof(Tree.performToolAction)),
                    AccessTools.Method(typeof(AggressiveAcorns), nameof(PerformToolAction_Prefix)),
                    PatchType.Prefix
                ),
                new HarmonyPatchInfo(
                    AccessTools.Method(typeof(Tree), nameof(Tree.dayUpdate)),
                    AccessTools.Method(typeof(AggressiveAcorns), nameof(DayUpdate_Prefix)),
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
                           __instance.growthStage.Value <= Config.MaxPassableGrowthStage;
            }
            catch (Exception ex)
            {
                ErrorLogger($"Failed in {nameof(IsPassable_Postfix)}:\n{ex}");
            }
        }


        public static bool PerformToolAction_Prefix(Tool t, ref bool __result)
        {
            try
            {
                if (!Config.DoMeleeWeaponsDestroySeedlings && t is MeleeWeapon)
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


        public static bool DayUpdate_Prefix(Tree __instance)
        {
            try
            {
                __instance.DayUpdateAggressively();
                return false; // Prevent other processing on the method.
            }
            catch (Exception ex)
            {
                ErrorLogger(
                    $"Harmony Patch failed in {nameof(DayUpdate_Prefix)}:\n{ex}"
                );
                return true; // Allow original method (& other patches) to run.
            }
        }
    }
}
