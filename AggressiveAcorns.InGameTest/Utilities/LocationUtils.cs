using Microsoft.Xna.Framework;
using StardewValley;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Utilities
{
    internal static class LocationUtils
    {
        public static readonly Warp WarpFarm = new Warp(0, 0, "farm", 63, 26, false);
        public static readonly Warp WarpDesert = new Warp(0, 0, "desert", 22, 48, false);
        public static readonly Warp WarpGreenhouse = new Warp(0, 0, "greenhouse", 9, 14, false);

        public static readonly Warp WarpBackwoods = new Warp(0, 0, "backwoods", 22, 14, false);
        public static readonly Warp WarpRailroad = new Warp(0, 0, "railroad", 36, 54, false);
        public static readonly Warp WarpMountain = new Warp(0, 0, "mountain", 18, 12, false);
        public static readonly Warp WarpFarmCave = new Warp(0, 0, "farmcave", 6, 8, false);
        public static readonly Warp WarpCellar = new Warp(0, 0, "cellar", 3, 10, false);
        public static readonly Warp WarpTown = new Warp(0, 0, "town", 65, 32, false);
        public static readonly Warp WarpBeach = new Warp(0, 0, "beach", 34, 13, false);
        public static readonly Warp WarpBusStop = new Warp(0, 0, "busstop", 14, 19, false);
        public static readonly Warp WarpWoods = new Warp(0, 0, "woods", 20, 18, false);
        public static readonly Warp WarpForest = new Warp(0, 0, "forest", 69, 17, false);


        public static GameLocation GetLocation(this Warp warp)
        {
            return Game1.getLocationFromName(warp.TargetName);
        }


        public static Vector2 GetTargetTile(this Warp warp)
        {
            return new Vector2(warp.TargetX, warp.TargetY);
        }


        public static void ClearLocation(this GameLocation location)
        {
            location.debris.Clear();
            location.objects.Clear();
            location.terrainFeatures.Clear();
            location.largeTerrainFeatures.Clear();
        }
    }
}
