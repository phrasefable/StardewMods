using System;
using JetBrains.Annotations;

namespace Phrasefable.StardewMods.StarUnit.Framework.Model
{
    public interface ITestSuite : ITraversableBranch<ITraversable>
    {
        [CanBeNull] public Action BeforeAll { get; }
        [CanBeNull] public Action BeforeEach { get; }
        [CanBeNull] public Action AfterEach { get; }
        [CanBeNull] public Action AfterAll { get; }
    }
}
