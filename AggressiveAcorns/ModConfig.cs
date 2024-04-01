using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns
{
    public class ModConfig : IModConfig
    {

        public bool DoMeleeWeaponsDestroySeedlings { get; set; } = false;

        public int MaxPassableGrowthStage { get; set; } = Tree.seedStage;

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
        public Dictionary<object, object> GrowthStages_INFO { get; private set; }
        public Dictionary<object, object> TreeTypes_INFO { get; private set; }

        // ===============================================================================


        public ModConfig()
        {
            this.ResetInfoEntries();
        }

        public void ResetInfoEntries()
        {
            this.Info_INFO = "All entries ending with '_INFO' will be reset each time the game is launched";
            this.GrowthStages_INFO = new Dictionary<object, object>
            {
                 {"Description", "Stage ID (integer) - use in config options"},
                 {"Seed", Tree.seedStage},
                 {"Sprout", Tree.sproutStage},
                 {"Sapling", Tree.saplingStage},
                 {"Bush", Tree.bushStage},
                 {"Still a bush", Tree.bushStage + 1},
                 {"Tree", Tree.treeStage},
                 {"Old enough to grow moss", Tree.stageForMossGrowth},
                 {"Maximum", Tree.stageForMossGrowth + 1},
            };

            string[] vanillaTreeTypes = [
                Tree.bushyTree, Tree.leafyTree, Tree.pineTree,
                Tree.winterTree1, Tree.winterTree2,
                Tree.palmTree, Tree.mushroomTree, Tree.mahoganyTree, Tree.palmTree2,
                Tree.greenRainTreeBushy, Tree.greenRainTreeLeafy, Tree.greenRainTreeFern, Tree.mysticTree
            ];

            this.TreeTypes_INFO = new Dictionary<object, object>
            {
                {"Tree type ID (string) - use in config options", ""},
                {Tree.bushyTree, "Oak"},
                {Tree.leafyTree, "Maple"},
                {Tree.pineTree, "Pine"},
                {Tree.palmTree, "Palm - Desert"},
                {Tree.mushroomTree, "Mushroom"},
                {Tree.mahoganyTree, "Mahogany"},
                {Tree.palmTree2, "Palm - Ginger Is."},
                {Tree.greenRainTreeBushy, "Green rain tree - transforms from Oak"},
                {Tree.greenRainTreeLeafy, "Green rain tree - transforms from Maple"},
                {Tree.greenRainTreeFern, "Green rain tree fern"},
                {Tree.mysticTree, "Mystic"},
            };

            Dictionary<string, StardewValley.GameData.WildTrees.WildTreeData> data = null;
            try
            {
                data = Tree.GetWildTreeDataDictionary();
            }
            catch (NullReferenceException) { }
            if (data is not null)
            {
                foreach (string key in data.Keys.Except(vanillaTreeTypes))
                {
                    this.TreeTypes_INFO.Add(key, "(modded)");
                }
            }

        }
    }


    public static class ModConfigUtils
    {

        public static ModConfig GetVanillaDefaults()
        {
            var Config = new ModConfig
            {
                MaxPassableGrowthStage = Tree.seedStage,
                DoMeleeWeaponsDestroySeedlings = true,
            };

            return Config;
        }
    }
}
