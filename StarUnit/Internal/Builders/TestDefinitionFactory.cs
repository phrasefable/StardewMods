using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using Phrasefable.StardewMods.StarUnit.Internal.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Builders
{
    internal class TestDefinitionFactory : ITestDefinitionFactory
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

        public ITestResult BuildTestResult(Status status)
        {
            return new TestResult {Status = status};
        }

        public ITestResult BuildTestResult(Status status, string message)
        {
            return new TestResult {Status = status, Message = message};
        }

        public IConditionDefinitions Conditions { get; } = new ConditionDefinitions();
    }
}
