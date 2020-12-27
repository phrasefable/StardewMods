namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Builders
{
    public interface IBuilderFactory
    {
        public ICasedTestBuilder<TCaseParams> CreateCasedTestBuilder<TCaseParams>();
        public ITestFixtureBuilder CreateFixtureBuilder();
        public ITestBuilder CreateTestBuilder();
    }
}