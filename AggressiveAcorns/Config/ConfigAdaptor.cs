using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;

namespace Phrasefable.StardewMods.AggressiveAcorns.Config
{
    internal class ConfigAdaptor : IConfigAdaptor
    {
        private readonly IModConfig _base;


        public ConfigAdaptor(IModConfig config)
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

        public bool RollForSpread => ConfigAdaptor.RandomChance(this._base.DailySpreadChance);
        public bool RollForGrowth => ConfigAdaptor.RandomChance(this._base.DailyGrowthChance);
        public bool RollForSeed => ConfigAdaptor.RandomChance(this._base.DailySeedChance);
        public bool RollForMushroomRegrowth => ConfigAdaptor.RandomChance(this._base.DailyGrowthChance / 2);

        public IEnumerable<Vector2> SpreadSeedOffsets => Framework.AggressiveTree.GenerateSpreadOffsets();

        private static bool RandomChance(double chance)
        {
            return Game1.random.NextDouble() < chance;
        }
    }
}
