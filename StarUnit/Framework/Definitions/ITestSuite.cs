using JetBrains.Annotations;

namespace Phrasefable.StardewMods.StarUnit.Framework.Definitions
{
    public interface ITestSuite : ITraversableBranch
    {
        [CanBeNull] public IAction BeforeAll { get; }
        [CanBeNull] public IAction BeforeEach { get; }
        [CanBeNull] public IAction AfterEach { get; }
        [CanBeNull] public IAction AfterAll { get; }
    }
}
