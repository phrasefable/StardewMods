using System.Linq;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Runners
{
    public interface IFixtureRunner
    {
        public ITestResult RunFixture(ITestFixture fixture, params string[] testKeys);
    }

    public class FixtureRunner : IFixtureRunner
    {
        public ITestResult RunFixture(ITestFixture fixture, params string[] testKeys)
        {
            var result = new TestResult(fixture);

            result.AggregateResults(
                from test in fixture.Tests
                where testKeys.Contains(test.Key)
                select test.RunTest()
            );

            return result;
        }
    }
}
