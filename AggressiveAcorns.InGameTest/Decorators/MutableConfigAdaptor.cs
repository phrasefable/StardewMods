using System;
using System.Diagnostics.CodeAnalysis;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Decorators
{
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class MutableConfigAdaptor : IConfigAdapter, IModConfig
    {
        private int _maxPassableGrowthStage;

        public MutableConfigAdaptor()
        {
            var config = new ModConfig();

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

            this.DailySpreadChance = config.DailySpreadChance;
            this.DailyGrowthChance = config.DailyGrowthChance;
            this.DailySeedChance = config.DailySeedChance;
        }

        public bool ProtectFromMelee { get; set; }
        public bool PreventScythe { get; set; }
        public bool SeedsReplaceGrass { get; set; }
        public int MaxShadedGrowthStage { get; set; }
        public double DailyGrowthChance { get; set; }
        public bool DoGrowInWinter { get; set; }
        public double DailySpreadChance { get; set; }
        public bool DoTappedSpread { get; set; }
        public bool DoSpreadInWinter { get; set; }
        public bool DoGrowInstantly { get; set; }
        public bool DoSeedsPersist { get; set; }
        public double DailySeedChance { get; set; }
        public bool DoMushroomTreesHibernate { get; set; }
        public bool DoMushroomTreesRegrow { get; set; }

        // ReSharper disable once ConvertToAutoPropertyWhenPossible
        public int MaxPassibleGrowthStage
        {
            get => this._maxPassableGrowthStage;
            set => this._maxPassableGrowthStage = value;
        }

        // ReSharper disable once ConvertToAutoPropertyWhenPossible
        public int MaxPassableGrowthStage
        {
            get => this._maxPassableGrowthStage;
            set => this._maxPassableGrowthStage = value;
        }

        public bool RollForSpread =>
            this.SpreadRoller?.Invoke() ?? ConfigAdapter.RandomChance(this.DailySpreadChance);

        public bool RollForGrowth =>
            this.GrowthRoller?.Invoke() ?? ConfigAdapter.RandomChance(this.DailyGrowthChance);

        public bool RollForSeed =>
            this.SeedRoller?.Invoke() ?? ConfigAdapter.RandomChance(this.DailySeedChance);

        public bool RollForMushroomRegrowth =>
            this.MushroomRegrowthRoller?.Invoke() ?? ConfigAdapter.RandomChance(this.DailyGrowthChance / 2);

        public Func<bool> SpreadRoller { private get; set; }
        public Func<bool> GrowthRoller { private get; set; }
        public Func<bool> SeedRoller { private get; set; }
        public Func<bool> MushroomRegrowthRoller { private get; set; }
    }
}
