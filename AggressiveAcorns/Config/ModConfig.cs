using System.Diagnostics.CodeAnalysis;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns.Config
{
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    [SuppressMessage(
        "ReSharper",
        "RedundantDefaultMemberInitializer",
        Justification = "Explicit default config values."
    )]
    public class ModConfig : IModConfig
    {
        public bool PreventScythe { get; set; } = false;

        public bool SeedsReplaceGrass { get; set; } = false;

        public int MaxShadedGrowthStage { get; set; } = Tree.treeStage - 1;

        public int MaxPassibleGrowthStage { get; set; } = Tree.seedStage;

        public double DailyGrowthChance { get; set; } = 0.20;

        public bool DoGrowInWinter { get; set; } = false;

        public double DailySpreadChance { get; set; } = 0.15;

        public bool DoTappedSpread { get; set; } = true;

        public bool DoSpreadInWinter { get; set; } = true;

        public bool DoGrowInstantly { get; set; } = false;

        public bool DoSeedsPersist { get; set; } = false;

        public double DailySeedChance { get; set; } = 0.05;

        public bool DoMushroomTreesHibernate { get; set; } = true;

        public bool DoMushroomTreesRegrow { get; set; } = false;
    }
}
