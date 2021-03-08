using System.Diagnostics.CodeAnalysis;

namespace Phrasefable.StardewMods.AggressiveAcorns.Config
{
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    public class ModConfig : IModConfig
    {
        public bool PreventScythe { get; set; } = false;

        public bool SeedsReplaceGrass { get; set; } = false;

        public int MaxShadedGrowthStage { get; set; } = 4;

        public int MaxPassibleGrowthStage { get; set; } = 0;

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
