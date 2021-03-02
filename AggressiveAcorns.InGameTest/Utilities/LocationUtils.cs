using Microsoft.Xna.Framework;
using StardewValley;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Utilities
{
    internal static class LocationUtils
    {
        public static readonly Warp WarpFarm = new Warp(0, 0, "farm", 63, 26, false);
        public static readonly Warp WarpDesert = new Warp(0, 0, "desert", 22, 48, false);
        public static readonly Warp WarpGreenhouse = new Warp(0, 0, "greenhouse", 9, 14, false);


        public static GameLocation GetLocation(this Warp warp)
        {
            return Game1.getLocationFromName(warp.TargetName);
        }


        public static Vector2 GetTargetTile(this Warp warp)
        {
            return new Vector2(warp.TargetX, warp.TargetY);
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
