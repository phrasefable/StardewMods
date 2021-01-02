using System;
using System.Collections.Generic;

namespace Phrasefable.StardewMods.StarUnit.Api.Model
{
    public interface IConditional
    {
        public IEnumerable<Func<Result>> Conditions { get; }
    }
}