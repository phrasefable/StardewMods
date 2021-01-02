using System;
using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Api;
using Phrasefable.StardewMods.StarUnit.Api.Model;

namespace Phrasefable.StardewMods.StarUnit.Default
{
    internal class Test : Identifiable, ITest
    {
        public IEnumerable<Func<Result>> Conditions { get; set; }

        public Func<Result> TestMethod { get; set; }
    }
}