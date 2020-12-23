using System;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Builders
{
    public interface ITestSuiteBuilder : INodeBuilder<ITestSuite, ITestSuiteBuilder>
    {
        public ITestSuiteBuilder AddTestSuite(Action<ITestSuiteBuilder> callback);
        public ITestSuiteBuilder AddTest(Action<ITestBuilder> callback);
        public ITestSuiteBuilder AddCasedTest<TCaseParams>(Action<ICasedTestBuilder<TCaseParams>> callback);
    }
}