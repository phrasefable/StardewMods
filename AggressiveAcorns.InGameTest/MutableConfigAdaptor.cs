using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class MutableConfigAdaptor : IConfigAdaptor
    {
        private readonly ModConfig _config;
        private readonly IConfigAdaptor _adaptor;


        public MutableConfigAdaptor()
        {
            this._config = new ModConfig();
            this._adaptor = new ConfigAdaptor(this._config);

            this.SpreadRoller = () => this._adaptor.RollForSpread;
            this.GrowthRoller = () => this._adaptor.RollForGrowth;
            this.SeedRoller = () => this._adaptor.RollForSeed;
            this.MushroomRegrowthRoller = () => this._adaptor.RollForMushroomRegrowth;
            this.SpreadOffsetGenerator = () => this._adaptor.SpreadSeedOffsets;
        }


        // ======= Overridable function members ========================================================================

        [NotNull] public Func<bool> SpreadRoller { private get; set; }
        [NotNull] public Func<bool> GrowthRoller { private get; set; }
        [NotNull] public Func<bool> SeedRoller { private get; set; }
        [NotNull] public Func<bool> MushroomRegrowthRoller { private get; set; }
        [NotNull] public Func<IEnumerable<Vector2>> SpreadOffsetGenerator { private get; set; }


        // ======= IConfigAdaptor members ==============================================================================

        public bool ProtectFromMelee
        {
            get => this._adaptor.ProtectFromMelee;
            set => this._config.PreventScythe = value;
        }

        public bool SeedsReplaceGrass
        {
            get => this._adaptor.SeedsReplaceGrass;
            set => this._config.SeedsReplaceGrass = value;
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

        public bool DoSeedsPersist
        {
            get => this._adaptor.DoSeedsPersist;
            set => this._config.DoSeedsPersist = value;
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

        public IEnumerable<Vector2> SpreadSeedOffsets => this.SpreadOffsetGenerator();
    }
}
