using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace Phrasefable.StardewMods.Common
{
    internal static class Utilities
    {
        [NotNull]
        public static IEnumerable<GameLocation> GetLocations(IModHelper helper)
        {
            if (Context.IsMainPlayer)
            {
                // From https://stardewvalleywiki.com/Modding:Common_tasks#Get_all_locations on 2019/03/16
                return Game1.locations.Concat(
                    from location in Game1.locations.OfType<BuildableGameLocation>()
                    from building in location.buildings
                    where building.indoors.Value != null
                    select building.indoors.Value
                );
            }

            return helper.Multiplayer.GetActiveLocations();
        }

        public static IEnumerable<Vector2> GetTilesInRadius(Vector2 centre, int radius)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    yield return centre + new Vector2(dx, dy);
                }
            }
        }
    }
}
