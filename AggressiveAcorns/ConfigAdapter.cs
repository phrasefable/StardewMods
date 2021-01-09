using StardewValley;

namespace Phrasefable.StardewMods.AggressiveAcorns
{
    internal class ConfigAdapter : IConfigAdapter
    {
        private readonly double _dailySpreadChance;
        private readonly double _dailyGrowthChance;
        private readonly double _dailySeedChance;

        public ConfigAdapter(IModConfig config)
        {
            this.ProtectFromMelee = config.PreventScythe;
            this.SeedsReplaceGrass = config.SeedsReplaceGrass;
            this.MaxShadedGrowthStage = config.MaxShadedGrowthStage;
            this.MaxPassableGrowthStage = config.MaxPassibleGrowthStage;
            this.DoGrowInWinter = config.DoGrowInWinter;
            this.DoTappedSpread = config.DoTappedSpread;
            this.DoSpreadInWinter = config.DoSpreadInWinter;
            this.DoGrowInstantly = config.DoGrowInstantly;
            this.DoSeedsPersist = config.DoSeedsPersist;
            this.DoMushroomTreesHibernate = config.DoMushroomTreesHibernate;
            this.DoMushroomTreesRegrow = config.DoMushroomTreesRegrow;

            this._dailySpreadChance = config.DailySpreadChance;
            this._dailyGrowthChance = config.DailyGrowthChance;
            this._dailySeedChance = config.DailySeedChance;
        }


        public bool ProtectFromMelee { get; }
        public bool SeedsReplaceGrass { get; }
        public int MaxShadedGrowthStage { get; }
        public int MaxPassableGrowthStage { get; }
        public bool DoGrowInWinter { get; }
        public bool DoTappedSpread { get; }
        public bool DoSpreadInWinter { get; }
        public bool DoGrowInstantly { get; }
        public bool DoSeedsPersist { get; }
        public bool DoMushroomTreesHibernate { get; }
        public bool DoMushroomTreesRegrow { get; }

        public bool RollForSpread => ConfigAdapter.RandomChance(this._dailySpreadChance);
        public bool RollForGrowth => ConfigAdapter.RandomChance(this._dailyGrowthChance);
        public bool RollForSeed => ConfigAdapter.RandomChance(this._dailySeedChance);
        public bool RollForMushroomRegrowth => ConfigAdapter.RandomChance(this._dailyGrowthChance / 2);


        internal static bool RandomChance(double chance)
        {
            return Game1.random.NextDouble() < chance;
        }
    }
}
