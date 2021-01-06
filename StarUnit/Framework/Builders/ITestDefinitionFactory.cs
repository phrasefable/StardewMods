namespace Phrasefable.StardewMods.StarUnit.Framework.Builders
{
    public interface ITestDefinitionFactory
    {
        public ICasedTestBuilder<TCaseParams> CreateCasedTestBuilder<TCaseParams>();
        public ITestFixtureBuilder CreateFixtureBuilder();
        public ITestBuilder CreateTestBuilder();

        public IResult BuildResult(Status status);
        public IResult BuildResult(Status status, string message);
    }
}
