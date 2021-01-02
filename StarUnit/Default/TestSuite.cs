using System;
using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Api;
using Phrasefable.StardewMods.StarUnit.Api.Model;

namespace Phrasefable.StardewMods.StarUnit.Default
{
    internal class TestSuite : Identifiable, ITestSuite
    {
        public IEnumerable<Func<Result>> Conditions { get; set; }

        public Action BeforeAll { get; set; }
        public Action BeforeEach { get; set; }
        public Action AfterEach { get; set; }
        public Action AfterAll { get; set; }

        public IEnumerable<ITraversable> Children { get; set; }
    }
}
