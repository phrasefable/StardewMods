using JetBrains.Annotations;

namespace Phrasefable.StardewMods.StarUnit.Framework.Definitions
{
    public interface IIdentifiable
    {
        public string Key { get; }
        [CanBeNull] public string LongName { get; }
    }
}
