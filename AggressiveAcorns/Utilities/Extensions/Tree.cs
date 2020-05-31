using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using SDVTree = StardewValley.TerrainFeatures.Tree;
using SDVGameLocation = StardewValley.GameLocation;

namespace AggressiveAcorns.Utilities.Extensions
{
    internal static class Tree
    {
        [Pure]
        public static bool MayGrowAt([NotNull] this SDVTree tree, [NotNull] SDVGameLocation location, Vector2 position)
        {
            var prop = location.doesTileHaveProperty((int) position.X, (int) position.Y, "NoSpawn", "Back");
            switch (prop)
            {
                case null:
                    break;
                case "All":
                case nameof(Tree):
                case "True":
                    return false;
            }

            return tree.growthStage.Value == SDVTree.seedStage && location.objects.ContainsKey(position);
        }


        // [Pure]
        // public static bool IsFullyGrown([NotNull] this SDVTree tree)
        // {
        //     return tree.growthStage.Value >= SDVTree.treeStage;
        // }


        [Pure]
        public static bool IsMushroomTree([NotNull] this SDVTree tree)
        {
            return tree.treeType.Value == SDVTree.mushroomTree;
        }
    }
}
