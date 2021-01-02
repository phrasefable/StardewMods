using System;
using Phrasefable.StardewMods.StarUnit.Api.Model;

namespace Phrasefable.StardewMods.StarUnit.Api.Builders
{
    public interface ITestFixtureBuilder : IBuilder<ITestSuite>, ITraversableBuilder
    {
        public Action BeforeAll { set; }
        public Action BeforeEach { set; }
        public Action AfterEach { set; }
        public Action AfterAll { set; }

        public void AddChild(ITraversable child);
    }
}