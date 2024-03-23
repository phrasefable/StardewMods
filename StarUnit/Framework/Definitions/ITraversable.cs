using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Framework.Definitions
{
    public interface ITraversable : IIdentifiable
    {
        IEnumerable<Func<IResult>> Conditions { get; }
        Delay Delay { get; }
    }
}
