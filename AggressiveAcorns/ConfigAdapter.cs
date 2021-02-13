using StardewValley;

namespace Phrasefable.StardewMods.AggressiveAcorns
{
    internal class ConfigAdapter : IConfigAdapter
    {
        private readonly IModConfig _base;


        public ConfigAdapter(IModConfig config)
        {
            this._base = config;
        }


        public bool ProtectFromMelee => this._base.PreventScythe;
        public bool SeedsReplaceGrass => this._base.SeedsReplaceGrass;
        public int MaxShadedGrowthStage => this._base.MaxShadedGrowthStage;
        public int MaxPassableGrowthStage => this._base.MaxPassibleGrowthStage;
        public bool DoGrowInWinter => this._base.DoGrowInWinter;
        public bool DoTappedSpread => this._base.DoTappedSpread;
        public bool DoSpreadInWinter => this._base.DoSpreadInWinter;
        public bool DoGrowInstantly => this._base.DoGrowInstantly;
        public bool DoSeedsPersist => this._base.DoSeedsPersist;
        public bool DoMushroomTreesHibernate => this._base.DoMushroomTreesHibernate;
        public bool DoMushroomTreesRegrow => this._base.DoMushroomTreesRegrow;

        public bool RollForSpread => ConfigAdapter.RandomChance(this._base.DailySpreadChance);
        public bool RollForGrowth => ConfigAdapter.RandomChance(this._base.DailyGrowthChance);
        public bool RollForSeed => ConfigAdapter.RandomChance(this._base.DailySeedChance);
        public bool RollForMushroomRegrowth => ConfigAdapter.RandomChance(this._base.DailyGrowthChance / 2);


        private static bool RandomChance(double chance)
        {
            return Game1.random.NextDouble() < chance;
        }
    }
}
