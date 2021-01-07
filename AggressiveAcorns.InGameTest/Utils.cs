using Microsoft.Xna.Framework;
using Phrasefable.StardewMods.Common;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest
{
    internal static class Utils
    {
        public static Warp WarpFarm = new Warp(0, 0, "farm", 63, 26, false);
        public static Warp WarpDesert = new Warp(0, 0, "desert", 22, 48, false);
        public static Warp WarpGreenhouse = new Warp(0, 0, "greenhouse", 9, 14, false);


        public static GameLocation GetLocation(this Warp warp)
        {
            return Game1.getLocationFromName(warp.TargetName);
        }


        public static Vector2 GetTargetTile(this Warp warp)
        {
            return new Vector2(warp.TargetX, warp.TargetY);
        }


        public static void Update(this Tree tree)
        {
            tree.dayUpdate(tree.currentLocation, tree.currentTileLocation);
        }


        public static Tree PlantTree(
            GameLocation location,
            Vector2 position,
            int treeType,
            int growthStage,
            bool ensureUnshaded = true
        )
        {
            if (ensureUnshaded)
            {
                foreach (Vector2 tile in Utilities.GetTilesInRadius(position, 3))
                {
                    location.removeEverythingExceptCharactersFromThisTile((int) tile.X, (int) tile.Y);
                }
            }

            Tree tree = new AggressiveTree(new Tree(treeType, growthStage));
            location.terrainFeatures.Add(position, tree);
            return tree;
        }


        public static void ClearLocation(GameLocation location)
        {
            location.debris.Clear();
            location.objects.Clear();
            location.terrainFeatures.Clear();
            location.largeTerrainFeatures.Clear();
        }
    }
}
