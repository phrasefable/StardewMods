namespace Phrasefable.StardewMods.AggressiveAcorns.Config
{
    public interface IModConfig
    {
        bool DoMeleeWeaponsDestroySeedlings { get; }

        int MaxPassableGrowthStage { get; }

        double ChanceGrowth { get; }
        int MaxShadedGrowthStage { get; }
        bool DoGrowInWinter { get; }
        bool DoGrowInstantly { get; }

        double ChanceSpread { get; }
        bool DoSeedsReplaceGrass { get; }
        bool DoTappedSpread { get; }
        bool DoSpreadInWinter { get; }

        double ChanceSeedGain { get; }
        double ChanceSeedLoss { get; }

        bool DoMushroomTreesHibernate { get; }
        bool DoMushroomTreesRegrow { get; }
    }
}
