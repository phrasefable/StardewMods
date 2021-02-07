using System;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Runners
{
    internal class TestSuiteRunner : BranchRunner<ITestSuite>
    {
        public TestSuiteRunner(ITraversableRunner childRunner) : base(childRunner) { }


        protected override ITraversableResult _Run(ITestSuite suite)
        {
            suite.BeforeAll?.Action.Invoke();

            IExecutionContext context = new SuiteChildContext(suite);
            ITraversableResult result = this.HandleChildren(
                suite,
                child => this.ChildRunner.Run(child, context),
                Status.Pass
            );

            suite.AfterAll?.Action.Invoke();

            return result;
        }


        protected override ITraversableResult _Run(ITestSuite suite, IExecutionContext context)
        {
            return context.Execute(suite, this.Run);
        }


        private class SuiteChildContext : IExecutionContext
        {
            private readonly ITestSuite _suite;


            public SuiteChildContext(ITestSuite suite)
            {
                this._suite = suite;
            }


            public ITraversableResult Execute(
                ITraversable traversable,
                Func<ITraversable, ITraversableResult> executable
            )
            {
                this._suite.BeforeEach?.Action.Invoke();
                ITraversableResult result = executable.Invoke(traversable);
                this._suite.AfterEach?.Action.Invoke();

                return result;
            }
        }
    }
}
