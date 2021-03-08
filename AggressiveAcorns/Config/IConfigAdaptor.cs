using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Phrasefable.StardewMods.AggressiveAcorns.Config
{
    internal interface IConfigAdaptor
    {
        public bool ProtectFromMelee { get; }
        public bool SeedsReplaceGrass { get; }
        public int MaxShadedGrowthStage { get; }
        public int MaxPassableGrowthStage { get; }
        public bool DoGrowInWinter { get; }
        public bool DoTappedSpread { get; }
        public bool DoSpreadInWinter { get; }
        public bool DoGrowInstantly { get; }
        public bool DoSeedsPersist { get; }
        public bool DoMushroomTreesHibernate { get; }
        public bool DoMushroomTreesRegrow { get; }

        public bool RollForSpread { get; }
        public bool RollForGrowth { get; }
        public bool RollForSeed { get; }
        public bool RollForMushroomRegrowth { get; }

        public IEnumerable<Vector2> SpreadSeedOffsets { get; }
    }
}
