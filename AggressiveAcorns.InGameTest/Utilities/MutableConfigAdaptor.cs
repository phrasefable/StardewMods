using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Phrasefable.StardewMods.AggressiveAcorns.Config;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Utilities
{
    internal class MutableConfigAdaptor : IConfigAdaptor
    {
        private readonly ModConfig _config;
        private readonly IConfigAdaptor _adaptor;


        public MutableConfigAdaptor()
        {
            this._config = new ModConfig();
            this._adaptor = new ConfigAdaptor(this._config);

            this.SpreadRoller = () => this._adaptor.RollForSpread;
            this.GrowthRoller = () => this._adaptor.RollForGrowth;
            this.GrowthMahoganyRoller = () => this._adaptor.RollForGrowthMahogany;
            this.GrowthMahoganyFertilizedRoller = () => this._adaptor.RollForGrowthMahoganyFertilized;
            this.SeedGainRoller = () => this._adaptor.RollForSeedGain;
            this.SeedLossRoller = () => this._adaptor.RollForSeedLoss;
            this.MushroomRegrowthRoller = () => this._adaptor.RollForMushroomRegrowth;
            this.SpreadOffsetGenerator = () => this._adaptor.SpreadSeedOffsets;
        }


        // ======= Overridable function members ========================================================================

        [JetBrains.Annotations.NotNull] public Func<bool> SpreadRoller { private get; set; }
        [JetBrains.Annotations.NotNull] public Func<bool> GrowthRoller { private get; set; }
        [JetBrains.Annotations.NotNull] public Func<bool> GrowthMahoganyRoller { private get; set; }
        [JetBrains.Annotations.NotNull] public Func<bool> GrowthMahoganyFertilizedRoller { private get; set; }
        [JetBrains.Annotations.NotNull] public Func<bool> SeedGainRoller { private get; set; }
        [JetBrains.Annotations.NotNull] public Func<bool> SeedLossRoller { private get; set; }
        [JetBrains.Annotations.NotNull] public Func<bool> MushroomRegrowthRoller { private get; set; }
        [JetBrains.Annotations.NotNull] public Func<IEnumerable<Vector2>> SpreadOffsetGenerator { private get; set; }


        // ======= IConfigAdaptor members ==============================================================================

        public bool DoMeleeWeaponsDestroySeedlings
        {
            get => this._adaptor.DoMeleeWeaponsDestroySeedlings;
            set => this._config.DoMeleeWeaponsDestroySeedlings = value;
        }

        public bool DoSeedsReplaceGrass
        {
            get => this._adaptor.DoSeedsReplaceGrass;
            set => this._config.DoSeedsReplaceGrass = value;
        }

        public int MaxShadedGrowthStage
        {
            get => this._adaptor.MaxShadedGrowthStage;
            set => this._config.MaxShadedGrowthStage = value;
        }

        public bool DoGrowInWinter
        {
            get => this._adaptor.DoGrowInWinter;
            set => this._config.DoGrowInWinter = value;
        }

        public bool DoTappedSpread
        {
            get => this._adaptor.DoTappedSpread;
            set => this._config.DoTappedSpread = value;
        }

        public bool DoSpreadInWinter
        {
            get => this._adaptor.DoSpreadInWinter;
            set => this._config.DoSpreadInWinter = value;
        }

        public bool DoGrowInstantly
        {
            get => this._adaptor.DoGrowInstantly;
            set => this._config.DoGrowInstantly = value;
        }

        public bool DoMushroomTreesHibernate
        {
            get => this._adaptor.DoMushroomTreesHibernate;
            set => this._config.DoMushroomTreesHibernate = value;
        }

        public bool DoMushroomTreesRegrow
        {
            get => this._adaptor.DoMushroomTreesRegrow;
            set => this._config.DoMushroomTreesRegrow = value;
        }

        public int MaxPassableGrowthStage
        {
            get => this._adaptor.MaxPassableGrowthStage;
            set => this._config.MaxPassableGrowthStage = value;
        }

        public double ChanceGrowth
        {
            set => this._config.ChanceGrowth = value;
        }

        public double ChanceGrowthMahogany
        {
            set => this._config.ChanceGrowthMahogany = value;
        }

        public double ChanceGrowthMahoganyFertilized
        {
            set => this._config.ChanceGrowthMahoganyFertilized = value;
        }

        public double ChanceSpread
        {
            set => this._config.ChanceSpread = value;
        }

        public double ChanceSeedGain
        {
            set => this._config.ChanceSeedGain = value;
        }

        public double ChanceSeedLoss
        {
            set => this._config.ChanceSeedLoss = value;
        }

        public bool RollForSpread => this.SpreadRoller();

        public bool RollForGrowth => this.GrowthRoller();

        public bool RollForGrowthMahogany => this.GrowthMahoganyRoller();

        public bool RollForGrowthMahoganyFertilized => this.GrowthMahoganyFertilizedRoller();

        public bool RollForSeedGain => this.SeedGainRoller();

        public bool RollForSeedLoss => this.SeedLossRoller();

        public bool RollForMushroomRegrowth => this.MushroomRegrowthRoller();

        public IEnumerable<Vector2> SpreadSeedOffsets => this.SpreadOffsetGenerator();
    }
}
