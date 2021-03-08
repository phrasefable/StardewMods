using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;

namespace Phrasefable.StardewMods.AggressiveAcorns
{
    internal static class TreeUtils
    {
        public static IEnumerable<Vector2> GetSpreadOffsets()
        {
            if (!AggressiveAcorns.Config.RollForSpread) yield break;

            static int GetOffset() => Game1.random.Next(-3, 4);
            yield return new Vector2(GetOffset(), GetOffset());
        }
    }
}
