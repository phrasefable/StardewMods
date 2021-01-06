using JetBrains.Annotations;

namespace Phrasefable.StardewMods.StarUnit.Framework.Model
{
    public interface IIdentifiable
    {
        public string Key { get; }
        [CanBeNull] public string LongName { get; }
    }
}
