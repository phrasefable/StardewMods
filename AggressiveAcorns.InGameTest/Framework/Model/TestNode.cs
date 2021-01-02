using System;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model
{
    public abstract class TestNode
    {
        private TestNode() { }

        public abstract TResult Match<TResult>(
            Func<ITestSuite, TResult> suiteHandler,
            Func<ITest, TResult> testHandler);

        public abstract void Switch(Action<ITestSuite> suiteHandler, Action<ITest> testHandler);

        public sealed class SuiteWrapper : TestNode
        {
            public ITestSuite Suite { get; }

            public SuiteWrapper(ITestSuite suite)
            {
                this.Suite = suite;
            }

            public override TResult Match<TResult>(
                Func<ITestSuite, TResult> suiteHandler,
                Func<ITest, TResult> testHandler)
            {
                return suiteHandler(Suite);
            }

            public override void Switch(Action<ITestSuite> suiteHandler, Action<ITest> testHandler)
            {
                suiteHandler(Suite);
            }
        }

        public sealed class TestWrapper : TestNode
        {
            public ITest Test { get; }

            public TestWrapper(ITest test)
            {
                this.Test = test;
            }

            public override TResult Match<TResult>(
                Func<ITestSuite, TResult> suiteHandler,
                Func<ITest, TResult> testHandler)
            {
                return testHandler(Test);
            }

            public override void Switch(Action<ITestSuite> suiteHandler, Action<ITest> testHandler)
            {
                testHandler(Test);
            }
        }
    }
}