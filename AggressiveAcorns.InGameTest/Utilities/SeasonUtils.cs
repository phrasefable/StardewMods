using StardewValley;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Utilities
{
    internal static class SeasonUtils
    {
        public static void SetSeason(Season season)
        {
            Game1.season = season;
            Game1.setGraphicsForSeason();
        }
    }
}
