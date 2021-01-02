using System;

namespace Phrasefable.StardewMods.StarUnit.Api.Model
{
    public interface ITestSuite : ITraversableBranch<ITraversable>
    {
        public Action BeforeAll { get; }
        public Action BeforeEach { get; }
        public Action AfterEach { get; }
        public Action AfterAll { get; }
    }
}