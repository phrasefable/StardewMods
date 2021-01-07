using System;
using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Definitions
{
    internal class Traversable : ITraversable
    {
        public string Key { get; set; }
        public string LongName { get; set; }

        IEnumerable<Func<IResult>> IConditional.Conditions => this.Conditions;
        public ICollection<Func<IResult>> Conditions { get; } = new List<Func<IResult>>();
    }
}