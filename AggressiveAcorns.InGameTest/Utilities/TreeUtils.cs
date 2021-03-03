using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Utilities
{
    internal static class TreeUtils
    {
        public static void Update(this Tree tree)
        {
            tree.dayUpdate(tree.currentLocation, tree.currentTileLocation);
        }


        public static Tree PlantTree(
            GameLocation location,
            Vector2 position,
            int treeType,
            int growthStage,
            bool ensureUnshaded = false
        )
        {
            if (ensureUnshaded)
            {
                foreach (Vector2 tile in Common.Utilities.GetTilesInRadius(position, 3))
                {
                    location.removeEverythingExceptCharactersFromThisTile((int) tile.X, (int) tile.Y);
                }
            }

            Tree tree = new AggressiveTree(new Tree(treeType, growthStage));
            location.terrainFeatures.Add(position, tree);
            return tree;
        }


        public static Tree GetFarmTreeLonely(int stage = Tree.treeStage, int type = Tree.pineTree)
        {
            GameLocation location = LocationUtils.WarpFarm.GetLocation();
            Vector2 position = LocationUtils.WarpFarm.GetTargetTile() + new Vector2(0, -2);

            LocationUtils.ClearLocation(location);
            return TreeUtils.PlantTree(location, position, type, stage);
        }
    }
}
