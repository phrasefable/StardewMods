namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Utilities
{
    static internal class StringUtils
    {
        public static string NormalizeNegatives(this string s)
        {
            return s.Replace("-", "neg_");
        }
    }
}