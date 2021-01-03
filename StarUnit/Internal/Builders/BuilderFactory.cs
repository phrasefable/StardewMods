using Phrasefable.StardewMods.StarUnit.Framework.Builders;

namespace Phrasefable.StardewMods.StarUnit.Internal.Builders
{
    public class BuilderFactory : IBuilderFactory
    {
        public ICasedTestBuilder<TCaseParams> CreateCasedTestBuilder<TCaseParams>()
        {
            return new CasedTestBuilder<TCaseParams>(this.CreateTestBuilder);
        }

        public ITestFixtureBuilder CreateFixtureBuilder()
        {
            return new TestFixtureBuilder();
        }

        public ITestBuilder CreateTestBuilder()
        {
            return new TestBuilder();
        }
    }
}