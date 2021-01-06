using System;
using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Model;

namespace Phrasefable.StardewMods.StarUnit.Internal.Model
{
    internal class TestGrouping : Identifiable, ITestGrouping
    {
        public IEnumerable<Func<IResult>> Conditions { get; set; }

        public IEnumerable<ITest> Children { get; set; }
    }
}
