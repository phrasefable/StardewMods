using System;

namespace Phrasefable.StardewMods.StarUnit.Framework.Model
{
    public interface ITestSuite : ITraversableBranch<ITraversable>
    {
        public Action BeforeAll { get; }
        public Action BeforeEach { get; }
        public Action AfterEach { get; }
        public Action AfterAll { get; }
    }
}