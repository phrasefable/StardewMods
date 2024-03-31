using StardewValley;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns.Framework
{
    internal static class TreeQueries
    {
        //public static bool TreeCanGrow(this Tree tree)
        //{
        //    string prop = tree.Location.doesTileHaveProperty((int) tree.Tile.X, (int) tree.Tile.Y, "NoSpawn", "Back");
        //    bool tileCanSpawnTree = prop == null || !(prop.Equals("All") || prop.Equals("Tree") || prop.Equals("True"));
        //    bool isBlockedSeed = tree.growthStage.Value == 0 && tree.Location.objects.ContainsKey(tree.Tile);
        //    return tileCanSpawnTree && !isBlockedSeed;
        //}

        public static bool IsFullyGrown(this Tree tree)
        {
            return tree.growthStage.Value >= AggressiveTree.MaxGrowthStage;
        }


        public static bool IsMushroomTree(this Tree tree)
        {
            return tree.treeType.Value == Tree.mushroomTree;
        }
    }
}
