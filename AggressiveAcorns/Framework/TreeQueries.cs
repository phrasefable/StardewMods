using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns.Framework
{
    internal static class TreeQueries
    {
        public static bool IsFullyGrown(this Tree tree)
        {
            return tree.growthStage.Value >= Tree.treeStage;
        }


        public static bool IsMushroomTree(this Tree tree)
        {
            return tree.treeType.Value == Tree.mushroomTree;
        }
    }
}
