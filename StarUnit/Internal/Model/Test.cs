using System;
using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Model;

namespace Phrasefable.StardewMods.StarUnit.Internal.Model
{
    internal class Test : Identifiable, ITest
    {
        public IEnumerable<Func<IResult>> Conditions { get; set; }

        public Func<IResult> TestMethod { get; set; }
    }
}
