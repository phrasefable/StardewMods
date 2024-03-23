using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Filterers.Wrappers
{
    internal class TestSuiteWrapperFactory : IBranchWrapperFactory<ITestSuite>
    {
        public ITestSuite Wrap(ITestSuite suite, IEnumerable<ITraversable> children)
        {
            return new TestSuiteWrapper(suite, children);
        }


        private class TestSuiteWrapper : ITestSuite
        {
            private readonly ITestSuite _testSuite;


            public TestSuiteWrapper(ITestSuite suite, IEnumerable<ITraversable> children)
            {
                this._testSuite = suite;
                this.Children = children;
            }


            public string Key => this._testSuite.Key;
            public string LongName => this._testSuite.LongName;
            public IEnumerable<Func<IResult>> Conditions => this._testSuite.Conditions;
            public Delay Delay => this._testSuite.Delay;
            public IAction BeforeAll => this._testSuite.BeforeAll;
            public IAction BeforeEach => this._testSuite.BeforeEach;
            public IAction AfterEach => this._testSuite.AfterEach;
            public IAction AfterAll => this._testSuite.AfterAll;

            public IEnumerable<ITraversable> Children { get; }
        }
    }
}