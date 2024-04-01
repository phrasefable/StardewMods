using GenericModConfigMenu;
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
        public const string Path_WildTreeData = "Data/WildTrees";


        private static Action<string> ErrorLogger;
        internal static ModConfig Config;

        internal const int MaxGrowthStage = Tree.stageForMossGrowth + 1;


        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();
            ErrorLogger = message => this.Monitor.Log(message, LogLevel.Error);

            this.ApplyPatches();

            helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
            helper.Events.Content.AssetReady += this.Content_AssetReady ;
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
            this.SetupGenericConfigMenu();
        }

        private void ReloadWildTreeDefinitions()
        {
            Config.ResetInfoEntries(Tree.GetWildTreeDataDictionary().Keys.ToArray());
            this.Helper.WriteConfig(Config);
        }

        private void SetupGenericConfigMenu()
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(Config)
            );

            configMenu.AddParagraph(
                mod: this.ModManifest,
                text: () => "Stages:\n0 = seed\n1 = sprout\n2 = sapling\n3..4 = bush\n5..13 = tree\n14..15 = tree, can grow moss"
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Melee weapons destroy saplings",
                getValue: () => Config.DoMeleeWeaponsDestroySeedlings,
                setValue: value => Config.DoMeleeWeaponsDestroySeedlings = value
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Max passable growth stage",
                getValue: () => Config.MaxPassableGrowthStage,
                setValue: value => Config.MaxPassableGrowthStage = value,
                min: -1,
                max: 15
            );
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
