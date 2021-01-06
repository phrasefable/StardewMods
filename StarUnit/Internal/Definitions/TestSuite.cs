using System;
using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Internal.Definitions
{
    internal class TestSuite : Traversable, ITestSuite
    {
        public Action BeforeAll { get; set; }
        public Action BeforeEach { get; set; }
        public Action AfterEach { get; set; }
        public Action AfterAll { get; set; }

        IEnumerable<ITraversable> ITestSuite.Children => Children;
        public ICollection<ITraversable> Children { get; } = new List<ITraversable>();
    }
}
