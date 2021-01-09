namespace Phrasefable.StardewMods.AggressiveAcorns
{
    public interface IConfigAdapter
    {
        bool ProtectFromMelee { get; }
        bool SeedsReplaceGrass { get; }
        int MaxShadedGrowthStage { get; }
        int MaxPassableGrowthStage { get; }
        bool DoGrowInWinter { get; }
        bool DoTappedSpread { get; }
        bool DoSpreadInWinter { get; }
        bool DoGrowInstantly { get; }
        bool DoSeedsPersist { get; }
        bool DoMushroomTreesHibernate { get; }
        bool DoMushroomTreesRegrow { get; }

        bool RollForSpread { get; }
        bool RollForGrowth { get; }
        bool RollForSeed { get; }
        bool RollForMushroomRegrowth { get; }
    }
}