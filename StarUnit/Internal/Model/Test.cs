using System;
using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Model;

namespace Phrasefable.StardewMods.StarUnit.Internal
{
    internal class Test : Identifiable, ITest
    {
        public IEnumerable<Func<Result>> Conditions { get; set; }

        public Func<Result> TestMethod { get; set; }
    }
}