using System.Collections.ObjectModel;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns
{
    public class ModConfig : IModConfig
    {

        public bool DoMeleeWeaponsDestroySeedlings { get; set; } = false;

        public int MaxPassableGrowthStage { get; set; } = Tree.seedStage;

        public string ChanceGrowth_INFO { get; private set; }
        public int ChanceGrowth { get; set; } = -1;
        public Dictionary<string, int> ChanceGrowth_Overrides { get; set; } = new Dictionary<string, int>() { { "ExampleTreeID", 99 }, { Tree.mahoganyTree, -1 }, { Tree.mysticTree, -1 } };
        public int ChanceGrowthFertilized { get; set; } = -1;
        public Dictionary<string, int> ChanceGrowthFertilized_Overrides { get; set; } = new Dictionary<string, int>() { { "ExampleTreeID", 20 }, { Tree.mahoganyTree, -1 }, { Tree.mysticTree, -1 } };


        //public double ChanceGrowth { get; set; } = 0.20;
        //public double ChanceGrowthMahogany { get; set; } = 0.15;
        //public double ChanceGrowthMahoganyFertilized { get; set; } = 0.60;
        //public int MaxShadedGrowthStage { get; set; } = Tree.treeStage - 1;
        //public bool DoGrowInWinter { get; set; } = false;
        //public bool DoGrowInstantly { get; set; } = false;

        //public double ChanceSpread { get; set; } = 0.15;
        //public bool DoSeedsReplaceGrass { get; set; } = false;
        //public bool DoTappedSpread { get; set; } = true;
        //public bool DoSpreadInWinter { get; set; } = true;

        //public double ChanceSeedGain { get; set; } = 0.05;
        //public double ChanceSeedLoss { get; set; } = 1.00;

        //public bool DoMushroomTreesHibernate { get; set; } = true;
        //public bool DoMushroomTreesRegrow { get; set; } = false;


        // ===============================================================================
        public string Info_INFO { get; private set; }
        public Dictionary<string, object> GrowthStages_INFO { get; private set; }
        public Dictionary<string, string> TreeTypes_INFO { get; private set; }

        // ===============================================================================


        public ModConfig()
        {
            this.ResetInfoEntries();
        }

        public void ResetInfoEntries(string[] wildTreeKeys = null)
        {
            this.Info_INFO = "All entries ending with '_INFO' will be reset each time the game is launched";
            this.ChanceGrowth_INFO = "'ChanceGrowth' will set the chance to grow for all trees, except those overriden in 'ChanceGrowth_Overrides'. 0 - 100 = 0% - 100%, set to -1 to not apply.";
            this.GrowthStages_INFO = new Dictionary<string, object>
            {
                 {"Description", "Stage ID (integer) - use in config options"},
                 {"Seed", Tree.seedStage},
                 {"Sprout", Tree.sproutStage},
                 {"Sapling", Tree.saplingStage},
                 {"Bush", Tree.bushStage},
                 {"Still a bush", Tree.bushStage + 1},
                 {"Tree", Tree.treeStage},
                 {"Old enough to grow moss", Tree.stageForMossGrowth},
                 {"Maximum", AggressiveAcorns.MaxGrowthStage},
            };

            string[] vanillaTreeTypes = [
                Tree.bushyTree, Tree.leafyTree, Tree.pineTree,
                Tree.winterTree1, Tree.winterTree2,
                Tree.palmTree, Tree.mushroomTree, Tree.mahoganyTree, Tree.palmTree2,
                Tree.greenRainTreeBushy, Tree.greenRainTreeLeafy, Tree.greenRainTreeFern, Tree.mysticTree
            ];

            this.TreeTypes_INFO = new Dictionary<string, string>
            {
                {"Tree type ID (string) - use in config options", ""},
            };

            foreach ((string key, string name) in  ModConfigUtils.TreeNames)
            {
                this.TreeTypes_INFO.Add(key, name);
            }

            if (wildTreeKeys is not null)
            {
                foreach (string key in wildTreeKeys.Except(vanillaTreeTypes))
                {
                    this.TreeTypes_INFO.Add(key, "(modded)");
                }
            }

        }
    }


    public static class ModConfigUtils
    {
        public static readonly ReadOnlyDictionary<string, string> TreeNames = new ReadOnlyDictionary<string, string>(
            new Dictionary<string, string>
            {
                    {Tree.bushyTree, "Oak"},
                    {Tree.leafyTree, "Maple"},
                    {Tree.pineTree, "Pine"},
                    {Tree.palmTree, "Desert Palm"},
                    {Tree.mushroomTree, "Mushroom"},
                    {Tree.mahoganyTree, "Mahogany"},
                    {Tree.palmTree2, "Ginger Is. Palm"},
                    {Tree.greenRainTreeBushy, "Green Rain Oak"},
                    {Tree.greenRainTreeLeafy, "Green Rain Maple"},
                    {Tree.greenRainTreeFern, "Green Rain Fern"},
                    {Tree.mysticTree, "Mystic"},
            }
        );

        public static ModConfig GetVanillaDefaults()
        {
            var Config = new ModConfig
            {
                MaxPassableGrowthStage = Tree.seedStage,
                DoMeleeWeaponsDestroySeedlings = true,
                ChanceGrowth = -1,
                ChanceGrowth_Overrides = [],
                ChanceGrowthFertilized = -1,
                ChanceGrowthFertilized_Overrides = [],
            };

            return Config;
        }
    }
}
