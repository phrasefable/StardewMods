using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;

namespace AggressiveAcorns.Utilities
{
    public static class TreeUtilities
    {
        public static IEnumerable<Vector2> GetSpreadLocations(this Vector2 position)
        {
            // pick random tile within +-3 x/y.
            var tileX = Game1.random.Next(-3, 4) + (int) position.X;
            var tileY = Game1.random.Next(-3, 4) + (int) position.Y;
            var seedPos = new Vector2(tileX, tileY);
            yield return seedPos;
        }
    }
}
