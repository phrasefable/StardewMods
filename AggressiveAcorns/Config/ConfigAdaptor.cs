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


        public bool DoMeleeWeaponsDestroySeedlings => this._base.DoMeleeWeaponsDestroySeedlings;
        public bool DoSeedsReplaceGrass => this._base.DoSeedsReplaceGrass;
        public int MaxShadedGrowthStage => this._base.MaxShadedGrowthStage;
        public int MaxPassableGrowthStage => this._base.MaxPassableGrowthStage;
        public bool DoGrowInWinter => this._base.DoGrowInWinter;
        public bool DoTappedSpread => this._base.DoTappedSpread;
        public bool DoSpreadInWinter => this._base.DoSpreadInWinter;
        public bool DoGrowInstantly => this._base.DoGrowInstantly;
        public bool DoMushroomTreesHibernate => this._base.DoMushroomTreesHibernate;
        public bool DoMushroomTreesRegrow => this._base.DoMushroomTreesRegrow;

        public bool RollForSpread => ConfigAdaptor.RandomChance(this._base.ChanceSpread);
        public bool RollForGrowth => ConfigAdaptor.RandomChance(this._base.ChanceGrowth);
        public bool RollForSeedGain => ConfigAdaptor.RandomChance(this._base.ChanceSeedGain);
        public bool RollForSeedLoss => ConfigAdaptor.RandomChance(this._base.ChanceSeedLoss);
        public bool RollForMushroomRegrowth => ConfigAdaptor.RandomChance(this._base.ChanceGrowth / 2);

        public IEnumerable<Vector2> SpreadSeedOffsets => Framework.AggressiveTree.GenerateSpreadOffsets();

        private static bool RandomChance(double chance)
        {
            return Game1.random.NextDouble() < chance;
        }
    }
}
