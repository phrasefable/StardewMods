using JetBrains.Annotations;
using Phrasefable.StardewMods.StarUnit.Framework.Model;

namespace Phrasefable.StardewMods.StarUnit.Framework
{
    public interface IResult : IIdentifiable
    {
        public Status Status { get; }
        [CanBeNull] public string Message { get; }
    }
}
