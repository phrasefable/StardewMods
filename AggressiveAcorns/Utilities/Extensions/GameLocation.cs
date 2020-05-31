using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using SDVGameLocation = StardewValley.GameLocation;
using SDVTree = StardewValley.TerrainFeatures.Tree;

namespace AggressiveAcorns.Utilities.Extensions
{
    internal static class GameLocation
    {
        [Pure]
        public static bool ExperiencingWinter([NotNull] this SDVGameLocation location)
        {
            return Game1.IsWinter && ExperiencesWinter(location);
        }


        [Pure]
        public static bool ExperiencesWinter([NotNull] this SDVGameLocation location)
        {
            return location.IsOutdoors && !(location is Desert);
        }


        [Pure]
        public static bool HasShadeAt([NotNull] this SDVGameLocation location, Vector2 position)
        {
            foreach (var adjacentTile in Utility.getSurroundingTileLocationsArray(position))
            {
                if (location.terrainFeatures.TryGetValue(adjacentTile, out var feature)
                    && feature is SDVTree adjTree
                    && adjTree.growthStage.Value == SDVTree.treeStage
                    && !adjTree.stump.Value)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
