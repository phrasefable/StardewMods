using JetBrains.Annotations;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns
{
    internal static class TreeExtensionsQueries
    {
        [Pure]
        public static bool IsFullyGrown(this Tree tree)
        {
            return tree.growthStage.Value >= Tree.treeStage;
        }


        [Pure]
        public static bool IsMushroomTree(this Tree tree)
        {
            return tree.treeType.Value == Tree.mushroomTree;
        }
    }
}
