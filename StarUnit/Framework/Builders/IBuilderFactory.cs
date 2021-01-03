namespace Phrasefable.StardewMods.StarUnit.Framework.Builders
{
    public interface IBuilderFactory
    {
        public ICasedTestBuilder<TCaseParams> CreateCasedTestBuilder<TCaseParams>();
        public ITestFixtureBuilder CreateFixtureBuilder();
        public ITestBuilder CreateTestBuilder();
    }
}