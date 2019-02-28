namespace AggressiveAcorns
{
    public class ModConfig
    {
        public bool PreventScythe { get; } = false;

        public bool SeedsReplaceGrass { get; } = false;

        public int MaxShadedGrowthStage { get; } = 4;

        public int MaxPassibleGrowthStage { get; } = 0;

        public double DailyGrowthChance { get; } = 0.20;

        public bool DoGrowInWinter { get; } = false;

        public double DailySpreadChance { get; } = 0.15;

        public bool DoTappedSpread { get; } = true;

        public bool DoSpreadInWinter { get; } = true;

        public bool DoGrowInstantly { get; } = false;

        public bool DoSeedsPersist { get; } = false;

        public double DailySeedChance { get; } = 0.05;

        public bool DoMushroomTreesHibernate { get; } = true;

        public bool DoMushroomTreesRegrow { get; } = false;
    }
}