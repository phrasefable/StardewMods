namespace Phrasefable.StardewMods.StarUnit.Framework.Definitions
{
    public interface ITestSuite : ITraversableBranch
    {
        public IAction BeforeAll { get; }
        public IAction BeforeEach { get; }
        public IAction AfterEach { get; }
        public IAction AfterAll { get; }
    }
}
