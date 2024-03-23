using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Utilities
{
    internal static class TreeUtils
    {
        // public static void Update(this Tree tree)
        // {
        //     tree.dayUpdate(tree.Location, tree.Tile);
        // }


        public static Tree PlantTree(
            GameLocation location,
            Vector2 position,
            string treeType,
            int growthStage,
            bool ensureUnshaded = false
        )
        {
            if (ensureUnshaded)
            {
                foreach (Vector2 tile in Common.Utilities.GetTilesInRadius(position, 3))
                {
                    location.removeObjectsAndSpawned((int) tile.X, (int) tile.Y, 1, 1);
                }
            }

            Tree tree = new Tree(treeType, growthStage);
            location.terrainFeatures.Add(position, tree);
            return tree;
        }


        public static Tree GetFarmTreeLonely(int stage = Tree.treeStage, string type = Tree.pineTree)
        {
            return TreeUtils.GetLonelyTree(LocationUtils.WarpFarm, stage, type);
        }


        public static Tree GetLonelyTree(Warp where, int stage = Tree.treeStage, string type = Tree.pineTree)
        {
            GameLocation location = where.GetLocation();
            Vector2 position = where.GetTargetTile() + new Vector2(0, -2);

            LocationUtils.ClearLocation(location);
            return TreeUtils.PlantTree(location, position, type, stage);
        }


        public static int[] Stages { get; } =
        {
            Tree.seedStage,
            Tree.sproutStage,
            Tree.saplingStage,
            Tree.bushStage,
            Tree.bushStage + 1,
            Tree.treeStage
        };
    }
}