using System.Collections.Generic;

namespace Phrasefable.StardewMods.StarUnit.Internal
{
    internal static class UtilityExtensions
    {
        public static void AddToValues<T>(this IDictionary<T, int> @this, IReadOnlyDictionary<T, int> other)
        {
            foreach (T key in other.Keys)
            {
                @this.AddToValue(key, other[key]);
            }
        }

        public static void AddToValue<T>(this IDictionary<T, int> @this, T key, int value)
        {
            if (@this.ContainsKey(key)) @this[key] += value;
            else @this[key] = value;
        }
    }
}
