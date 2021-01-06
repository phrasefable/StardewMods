using System;
using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Definitions
{
    internal class Test : Identifiable, ITest
    {
        public IEnumerable<Func<IResult>> Conditions { get; set; }

        public Func<ITestResult> TestMethod { get; set; }
    }
}
