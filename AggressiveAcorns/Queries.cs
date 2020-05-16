using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace AggressiveAcorns
{
    internal static class Queries
    {
        public static IEnumerable<Vector2> GetSpreadLocations(Vector2 position)
        {
            // pick random tile within +-3 x/y.
            int tileX = Game1.random.Next(-3, 4) + (int) position.X;
            int tileY = Game1.random.Next(-3, 4) + (int) position.Y;
            var seedPos = new Vector2(tileX, tileY);
            yield return seedPos;
        }


        public static bool TreeCanGrow(Tree tree, GameLocation location, Vector2 position)
        {
            string prop = location.doesTileHaveProperty((int) position.X, (int) position.Y, "NoSpawn", "Back");
            bool tileCanSpawnTree = prop == null || !(prop.Equals("All") || prop.Equals("Tree") || prop.Equals("True"));
            bool isBlockedSeed = tree.growthStage.Value == 0 && location.objects.ContainsKey(position);
            return tileCanSpawnTree && !isBlockedSeed;
        }


        public static bool ExperiencingWinter(GameLocation location)
        {
            return Game1.IsWinter && ExperiencesWinter(location);
        }


        public static bool ExperiencesWinter(GameLocation location)
        {
            return location.IsOutdoors && !(location is Desert);
        }


        public static bool IsShaded(GameLocation location, Vector2 position)
        {
            foreach (Vector2 adjacentTile in Utility.getSurroundingTileLocationsArray(position))
            {
                if (location.terrainFeatures.TryGetValue(adjacentTile, out TerrainFeature feature)
                    && feature is Tree adjTree
                    && IsFullyGrown(adjTree)
                    && !adjTree.stump.Value)
                {
                    return true;
                }
            }

            return false;
        }


        public static bool IsFullyGrown(Tree tree)
        {
            return tree.growthStage.Value >= Tree.treeStage;
        }


        public static bool IsMushroomTree(Tree tree)
        {
            return tree.treeType.Value == Tree.mushroomTree;
        }
    }
}
