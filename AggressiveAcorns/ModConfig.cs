using System.Diagnostics.CodeAnalysis;
using StardewValley.TerrainFeatures;

namespace AggressiveAcorns
{
    public interface IModConfig
    {
        bool DoScythesDestroySeedlings { get; }

        bool DoSeedsReplaceGrass { get; }

        int MaxShadedGrowthStage { get; }

        int MaxPassibleGrowthStage { get; }

        double DailyChanceGrowth { get; }

        bool DoGrowInWinter { get; }

        double DailyChanceSpread { get; }

        bool DoTappedSpread { get; }

        bool DoSpreadInWinter { get; }

        bool DoGrowInstantly { get; }

        double DailyChanceSeedLoss { get; }

        double DailyChanceSeedGain { get; }

        bool DoMushroomTreesHibernate { get; }

        bool DoMushroomTreesRegrow { get; }
    }

    /// <summary>
    /// Mod config - properties default to vanilla values.
    /// </summary>
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    public class ModConfig : IModConfig
    {
        public bool DoScythesDestroySeedlings { get; set; } = true;

        public bool DoSeedsReplaceGrass { get; set; } = false;

        public int MaxShadedGrowthStage { get; set; } = 4;

        public int MaxPassibleGrowthStage { get; set; } = Tree.seedStage;

        public double DailyChanceGrowth { get; set; } = 0.20;

        public bool DoGrowInWinter { get; set; } = false;

        public double DailyChanceSpread { get; set; } = 0.15;

        public bool DoTappedSpread { get; set; } = true;

        public bool DoSpreadInWinter { get; set; } = true;

        public bool DoGrowInstantly { get; set; } = false;

        public double DailyChanceSeedLoss { get; set; } = 1.00;

        public double DailyChanceSeedGain { get; set; } = Tree.chanceForDailySeed;

        public bool DoMushroomTreesHibernate { get; set; } = true;

        public bool DoMushroomTreesRegrow { get; set; } = false;
    }
}
