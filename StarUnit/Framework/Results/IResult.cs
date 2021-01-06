using JetBrains.Annotations;

namespace Phrasefable.StardewMods.StarUnit.Framework.Results
{
    public interface IResult
    {
        public Status Status { get; }
        [CanBeNull] public string Message { get; }
    }
}
