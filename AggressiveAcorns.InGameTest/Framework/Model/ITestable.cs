using JetBrains.Annotations;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model
{
    public interface ITestable
    {
        [NotNull] public IResult Invoke();
    }
}