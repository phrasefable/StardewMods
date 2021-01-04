using System;
using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Model;

namespace Phrasefable.StardewMods.StarUnit.Internal
{
    internal class TestGrouping : Identifiable, ITestGrouping
    {
        public IEnumerable<Func<Result>> Conditions { get; set; }

        public IEnumerable<ITest> Children { get; set; }
    }
}