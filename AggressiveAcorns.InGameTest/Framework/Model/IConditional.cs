using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model
{
    public interface IConditional
    {
        [NotNull] public IEnumerable<Func<IResult>> Conditions { get; }
    }
}