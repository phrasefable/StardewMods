using System;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Builders
{
    public interface ITestSuiteBuilder : INodeBuilder<ITestSuite, ITestSuiteBuilder>
    {
        public ITestSuiteBuilder AddTestSuite(string key, Action<ITestSuiteBuilder> callback);
        public ITestSuiteBuilder AddTest(string key, Action<ITestBuilder> callback);
        public ITestSuiteBuilder AddCasedTest<TCaseParams>(string key, Action<ICasedTestBuilder<TCaseParams>> callback);

        public ICasedTestBuilder<TCaseParams> AddCasedTest<TCaseParams>(string key);
    }
}
