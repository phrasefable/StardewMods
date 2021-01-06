using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Framework.Builders
{
    public interface ITestDefinitionFactory
    {
        public ICasedTestBuilder<TCaseParams> CreateCasedTestBuilder<TCaseParams>();
        public ITestFixtureBuilder CreateFixtureBuilder();
        public ITestBuilder CreateTestBuilder();

        public ITestResult BuildTestResult(Status status);
        public ITestResult BuildTestResult(Status status, string message);

        public IConditionDefinitions Conditions { get; }
    }
}
