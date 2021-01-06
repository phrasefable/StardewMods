using System;
using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Definitions
{
    internal class TestSuite : Identifiable, ITestSuite
    {
        public IEnumerable<Func<IResult>> Conditions { get; set; }

        public Action BeforeAll { get; set; }
        public Action BeforeEach { get; set; }
        public Action AfterEach { get; set; }
        public Action AfterAll { get; set; }

        public IEnumerable<ITraversable> Children { get; set; }
    }
}
