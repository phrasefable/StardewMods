using System;
using System.Collections.Generic;

namespace Phrasefable.StardewMods.StarUnit.Framework.Model
{
    public interface IConditional
    {
        public IEnumerable<Func<IResult>> Conditions { get; }
    }
}