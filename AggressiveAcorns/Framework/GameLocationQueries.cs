using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns.Framework
{
    internal static class GameLocationQueries
    {
        [Pure]
        public static bool TreeCanGrowAt(this GameLocation location, Tree tree, Vector2 position)
        {
            string prop = location.doesTileHaveProperty((int) position.X, (int) position.Y, "NoSpawn", "Back");
            bool tileCanSpawnTree = prop == null || !(prop.Equals("All") || prop.Equals("Tree") || prop.Equals("True"));
            bool isBlockedSeed = tree.growthStage.Value == 0 && location.objects.ContainsKey(position);
            return tileCanSpawnTree && !isBlockedSeed;
        }


        [Pure]
        public static bool ExperiencingWinter(this GameLocation location)
        {
            return Game1.GetSeasonForLocation(location) == "winter" && location.ExperiencesWinter();
        }


        [Pure]
        public static bool ExperiencesWinter(this GameLocation location)
        {
            // Override takes highest priority, in case it _is_ winter.
            if (!string.IsNullOrWhiteSpace(location.seasonOverride))
            {
                return location.seasonOverride == "winter";
            }

            if (!location.IsOutdoors && !location.treatAsOutdoors.Value) return false;
            if (location is Desert) return false;
            return !location.SeedsIgnoreSeasonsHere();
        }


        [Pure]
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
