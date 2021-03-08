namespace Phrasefable.StardewMods.AggressiveAcorns.Config
{
    public interface IModConfig
    {
        bool PreventScythe { get; }

        bool SeedsReplaceGrass { get; }

        int MaxShadedGrowthStage { get; }

        int MaxPassibleGrowthStage { get; } // TODO spellcheck pass_i_ble -> pass_a_ble

        double DailyGrowthChance { get; }

        bool DoGrowInWinter { get; }

        double DailySpreadChance { get; }

        bool DoTappedSpread { get; }

        bool DoSpreadInWinter { get; }

        bool DoGrowInstantly { get; }

        bool DoSeedsPersist { get; }

        double DailySeedChance { get; }

        bool DoMushroomTreesHibernate { get; }

        bool DoMushroomTreesRegrow { get; }
    }
}
