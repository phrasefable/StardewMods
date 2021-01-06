using System;
using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Framework.Model;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Model
{
    internal class Test : Identifiable, ITest
    {
        public IEnumerable<Func<IResult>> Conditions { get; set; }

        public Func<ITestResult> TestMethod { get; set; }
    }
}
