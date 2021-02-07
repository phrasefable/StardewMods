using Phrasefable.StardewMods.StarUnit.Framework.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Internal.Definitions
{
    internal class TestSuite : TraversableBranch, ITestSuite
    {
        public IAction BeforeAll { get; set; }
        public IAction BeforeEach { get; set; }
        public IAction AfterEach { get; set; }
        public IAction AfterAll { get; set; }
    }
}