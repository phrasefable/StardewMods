using System;
using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Model;

namespace Phrasefable.StardewMods.StarUnit.Internal.Model
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
