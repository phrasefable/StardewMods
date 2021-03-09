using System.Diagnostics.CodeAnalysis;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns.Config
{
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    [SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer", Justification = "Explicit default values.")]
    public class ModConfig : IModConfig
    {
        public bool DoMeleeWeaponsDestroySeedlings { get; set; } = false;

        public int MaxPassableGrowthStage { get; set; } = Tree.seedStage;

        public double ChanceGrowth { get; set; } = 0.20;
        public double ChanceGrowthMahogany { get; set; } = 0.15;
        public double ChanceGrowthMahoganyFertilized { get; set; } = 0.60;
        public int MaxShadedGrowthStage { get; set; } = Tree.treeStage - 1;
        public bool DoGrowInWinter { get; set; } = false;
        public bool DoGrowInstantly { get; set; } = false;

        public double ChanceSpread { get; set; } = 0.15;
        public bool DoSeedsReplaceGrass { get; set; } = false;
        public bool DoTappedSpread { get; set; } = true;
        public bool DoSpreadInWinter { get; set; } = true;

        public double ChanceSeedGain { get; set; } = 0.05;
        public double ChanceSeedLoss { get; set; } = 1.00;

        public bool DoMushroomTreesHibernate { get; set; } = true;
        public bool DoMushroomTreesRegrow { get; set; } = false;
    }
}
