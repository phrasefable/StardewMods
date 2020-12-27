using JetBrains.Annotations;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model
{
    public interface IIdentifiable
    {
        /// <summary>
        /// Short identification key, may contain only letters, numbers, '-' and '_'.
        /// </summary>
        [NotNull] public string Key { get; }

        /// <summary>
        /// Free-form field, can be used if <code>Key</code> is not descriptive enough
        /// </summary>
        [CanBeNull] public string LongName { get; }
    }
}