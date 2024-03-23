using StardewValley;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Utilities
{
    internal static class SeasonUtils
    {
        public static void SetSeason(string seasonName)
        {
            Game1.currentSeason = seasonName;
            Game1.setGraphicsForSeason();
        }
    }


    internal enum Season
    {
        Spring,
        Summer,
        Fall,
        Winter
    }


    internal static class SeasonsExtensions
    {
        public static string GetName(this Season season)
        {
            return season switch
            {
                Season.Spring => "spring",
                Season.Summer => "summer",
                Season.Fall => "fall",
                Season.Winter => "winter",
                _ => throw new ArgumentOutOfRangeException(nameof(season), season, null)
            };
        }

        public static void SetSeason(this Season season)
        {
            SeasonUtils.SetSeason(season.GetName());
        }
    }
}
