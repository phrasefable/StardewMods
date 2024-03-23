using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns.Framework
{
    internal static class GameLocationQueries
    {
        public static bool ExperiencingWinter(this GameLocation location)
        {
            return Game1.GetSeasonForLocation(location) == Season.Winter && location.ExperiencesWinter();
        }


        public static bool ExperiencesWinter(this GameLocation location)
        {
            if (!location.IsOutdoors && !location.treatAsOutdoors.Value) return false;
            if (location is Desert) return false;
            return !location.SeedsIgnoreSeasonsHere();
        }


        public static bool IsShadedAt(this GameLocation location, Vector2 position)
        {
            foreach (Vector2 adjacentTile in Utility.getSurroundingTileLocationsArray(position))
            {
                if (location.terrainFeatures.TryGetValue(adjacentTile, out TerrainFeature feature)
                    && feature is Tree adjTree
                    && adjTree.IsFullyGrown()
                    && !adjTree.stump.Value)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
