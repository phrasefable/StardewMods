using System;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Internal.Definitions
{
    internal class TestSuite : TraversableBranch, ITestSuite
    {
        public Action BeforeAll { get; set; }
        public Action BeforeEach { get; set; }
        public Action AfterEach { get; set; }
        public Action AfterAll { get; set; }
    }
}