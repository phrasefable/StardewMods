using System;
using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Model;

namespace Phrasefable.StardewMods.StarUnit.Internal
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