using System;
using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Framework.Definitions
{
    public interface IConditional
    {
        public IEnumerable<Func<IResult>> Conditions { get; }
    }
}
