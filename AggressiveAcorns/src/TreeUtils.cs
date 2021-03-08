using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns
{
    internal static class TreeUtils
    {
        public static IEnumerable<Vector2> GetSpreadOffsets()
        {
            if (!AggressiveAcorns.Config.RollForSpread) yield break;

            static int GetOffset() => Game1.random.Next(-3, 4);
            yield return new Vector2(GetOffset(), GetOffset());
        }


        public static bool TreeCanGrowAt(this GameLocation location, Tree tree, Vector2 position)
        {
            string prop = location.doesTileHaveProperty((int) position.X, (int) position.Y, "NoSpawn", "Back");
            bool tileCanSpawnTree = prop == null || !(prop.Equals("All") || prop.Equals("Tree") || prop.Equals("True"));
            bool isBlockedSeed = tree.growthStage.Value == 0 && location.objects.ContainsKey(position);
            return tileCanSpawnTree && !isBlockedSeed;
        }


        public static bool ExperiencingWinter(this GameLocation location)
        {
            return Game1.IsWinter && location.ExperiencesWinter();
        }


        public static bool ExperiencesWinter(this GameLocation location)
        {
            return location.IsOutdoors && !(location is Desert);
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
