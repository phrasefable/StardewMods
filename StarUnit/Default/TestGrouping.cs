using System;
using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Api;
using Phrasefable.StardewMods.StarUnit.Api.Model;

namespace Phrasefable.StardewMods.StarUnit.Default
{
    internal class TestGrouping : Identifiable, ITestGrouping
    {
        public IEnumerable<Func<Result>> Conditions { get; set; }

        public IEnumerable<ITest> Children { get; set; }
    }
}