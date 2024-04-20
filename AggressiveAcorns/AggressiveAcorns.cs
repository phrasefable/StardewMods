using GenericModConfigMenu;
using HarmonyLib;
using Phrasefable.StardewMods.Common.Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.WildTrees;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace Phrasefable.StardewMods.AggressiveAcorns
{
    public class AggressiveAcorns : Mod
    {
        public const string Path_WildTreeData = "Data/WildTrees";


        internal static Action<string> ErrorLogger;
        internal static ModConfig Config;

        internal const int MaxGrowthStage = Tree.stageForMossGrowth + 1;


        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();
            ErrorLogger = message => this.Monitor.Log(message, LogLevel.Error);

            this.ApplyPatches();

            helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
            helper.Events.Content.AssetReady += this.Content_AssetReady;
            helper.Events.Content.AssetRequested += this.Content_AssetRequested;
        }

        private void Content_AssetReady(object sender, StardewModdingAPI.Events.AssetReadyEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo(Path_WildTreeData))
            {
                this.ReloadWildTreeDefinitions();
            }
        }

        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            this.ReloadWildTreeDefinitions();

            var configApi = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configApi is not null)
            {
                new ConfigMenu(configApi, this.ModManifest, Config)
                    .Build(
                        saveConfig: () => this.Helper.WriteConfig(Config),
                        resetConfig: () => Config = new ModConfig()
                    );
            }

        }

        private void ReloadWildTreeDefinitions()
        {
            Config.ResetInfoEntries(Tree.GetWildTreeDataDictionary().Keys.ToArray());
            this.Helper.WriteConfig(Config);
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


        private void Content_AssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (!e.NameWithoutLocale.IsEquivalentTo(Path_WildTreeData)) return;
            e.Edit(asset => 
                {
                    var wildTreeDefs = asset.AsDictionary<string, WildTreeData>().Data;

                    foreach ((string treeId, WildTreeData wildTreeData) in wildTreeDefs)
                    {
                        SetOverridableChance(treeId, Config.ChanceGrowth, Config.ChanceGrowth_Overrides, chance => wildTreeData.GrowthChance = chance);
                        SetOverridableChance(treeId, Config.ChanceGrowthFertilized, Config.ChanceGrowthFertilized_Overrides, chance => wildTreeData.FertilizedGrowthChance = chance);
                    }
                }
            );

        }


        private static void SetOverridableChance(string treeId, int baseChance, Dictionary<string, int> overrides, Action<float> setter)
        {
            int chance = overrides.ContainsKey(treeId) ? overrides[treeId] : baseChance;
            if (chance >= 0)
            {
                setter((float) chance / 100);
            }
        }
    }
}
