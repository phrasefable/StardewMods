using System;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Framework.Builders
{
    public interface ITestFixtureBuilder : IBuilder<ITestSuite>, ITraversableBuilder
    {
        public Action BeforeAll { set; }
        public Action BeforeEach { set; }
        public Action AfterEach { set; }
        public Action AfterAll { set; }

        public Delay BeforeAllDelay { set; }
        public Delay BeforeEachDelay { set; }
        public Delay AfterEachDelay { set; }
        public Delay AfterAllDelay { set; }

        public void AddChild(ITraversable child);
    }
}
