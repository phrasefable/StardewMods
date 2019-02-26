namespace AggressiveAcorns
{
    internal interface IModConfig
    {
        bool bPreventScythe { get; }

        bool bSeedsReplaceGrass { get; }

        int iMaxShadedGrowthStage { get; }

        int iMaxPassibleGrowthStage { get; }

        bool bDoGrowInWinter { get; }

        bool bDoSpreadInWinter { get; }

        double fDailyGrowthChance { get; }

        bool bDoGrowInstantly { get; }

        bool bDoSeedsPersist { get; }

        bool bDoTappedSpread { get; }

        bool bDoMushroomTreesHibernate { get; }
    }

    public class ModConfig : IModConfig
    {
        public bool bPreventScythe { get; set; } = true;

        public bool bSeedsReplaceGrass { get; set; } = true;

        public int iMaxShadedGrowthStage { get; set; } = 4;

        public int iMaxPassibleGrowthStage { get; set; } = 1;

        public bool bDoGrowInWinter { get; set; } = false;

        public bool bDoSpreadInWinter { get; set; } = true;

        public double fDailyGrowthChance { get; set; } = 0.20;

        public bool bDoGrowInstantly { get; set; } = false;

        public bool bDoSeedsPersist { get; set; } = true;

        public bool bDoTappedSpread { get; set; } = true;

        public bool bDoMushroomTreesHibernate { get; set; } = true;
    }
}