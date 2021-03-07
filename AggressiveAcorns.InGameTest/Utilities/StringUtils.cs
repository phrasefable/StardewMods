namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Utilities
{
    static internal class StringUtils
    {
        public static string NormalizeNegatives(string s)
        {
            return s.Replace("-", "neg_");
        }
    }
}