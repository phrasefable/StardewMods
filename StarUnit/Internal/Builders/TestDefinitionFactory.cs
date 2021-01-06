using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;

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

        public IResult BuildResult(Status status)
        {
            return new Result {Status = status};
        }

        public IResult BuildResult(Status status, string message)
        {
            return new Result {Status = status, Message = message};
        }

        public IConditionDefinitions Conditions { get; } = new ConditionDefinitions();
    }
}
