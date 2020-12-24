using JetBrains.Annotations;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model
{
    public interface IResult
    {
        [NotNull] public Status Status { get; }
        [CanBeNull] public string Message { get; }
    }
}