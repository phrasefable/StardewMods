using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class MutableConfigAdaptor : IConfigAdapter
    {
        private readonly ModConfig _config;
        private readonly IConfigAdapter _adapter;


        public MutableConfigAdaptor()
        {
            this._config = new ModConfig();
            this._adapter = new ConfigAdapter(this._config);

            this.SpreadRoller = () => this._adapter.RollForSpread;
            this.GrowthRoller = () => this._adapter.RollForGrowth;
            this.SeedRoller = () => this._adapter.RollForSeed;
            this.MushroomRegrowthRoller = () => this._adapter.RollForMushroomRegrowth;
        }


        // ======= Overridable function members ========================================================================

        [NotNull] public Func<bool> SpreadRoller { private get; set; }
        [NotNull] public Func<bool> GrowthRoller { private get; set; }
        [NotNull] public Func<bool> SeedRoller { private get; set; }
        [NotNull] public Func<bool> MushroomRegrowthRoller { private get; set; }


        // ======= IConfigAdapter members ==============================================================================

        public bool ProtectFromMelee
        {
            get => this._adapter.ProtectFromMelee;
            set => this._config.PreventScythe = value;
        }

        public bool SeedsReplaceGrass
        {
            get => this._adapter.SeedsReplaceGrass;
            set => this._config.SeedsReplaceGrass = value;
        }

        public int MaxShadedGrowthStage
        {
            get => this._adapter.MaxShadedGrowthStage;
            set => this._config.MaxShadedGrowthStage = value;
        }

        public bool DoGrowInWinter
        {
            get => this._adapter.DoGrowInWinter;
            set => this._config.DoGrowInWinter = value;
        }

        public bool DoTappedSpread
        {
            get => this._adapter.DoTappedSpread;
            set => this._config.DoTappedSpread = value;
        }

        public bool DoSpreadInWinter
        {
            get => this._adapter.DoSpreadInWinter;
            set => this._config.DoSpreadInWinter = value;
        }

        public bool DoGrowInstantly
        {
            get => this._adapter.DoGrowInstantly;
            set => this._config.DoGrowInstantly = value;
        }

        public bool DoSeedsPersist
        {
            get => this._adapter.DoSeedsPersist;
            set => this._config.DoSeedsPersist = value;
        }

        public bool DoMushroomTreesHibernate
        {
            get => this._adapter.DoMushroomTreesHibernate;
            set => this._config.DoMushroomTreesHibernate = value;
        }

        public bool DoMushroomTreesRegrow
        {
            get => this._adapter.DoMushroomTreesRegrow;
            set => this._config.DoMushroomTreesRegrow = value;
        }

        public int MaxPassableGrowthStage
        {
            get => this._adapter.MaxPassableGrowthStage;
            set => this._config.MaxPassibleGrowthStage = value;
        }

        public double DailyGrowthChance
        {
            set => this._config.DailyGrowthChance = value;
        }

        public double DailySpreadChance
        {
            set => this._config.DailySpreadChance = value;
        }

        public double DailySeedChance
        {
            set => this._config.DailySeedChance = value;
        }

        public bool RollForSpread => this.SpreadRoller();

        public bool RollForGrowth => this.GrowthRoller();

        public bool RollForSeed => this.SeedRoller();

        public bool RollForMushroomRegrowth => this.MushroomRegrowthRoller();
    }
}
