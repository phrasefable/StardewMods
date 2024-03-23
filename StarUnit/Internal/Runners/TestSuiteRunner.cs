using System;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Internal.Runners
{
    internal class TestSuiteRunner : BranchRunner<ITestSuite>
    {
        public TestSuiteRunner(IRunnerDelegator delegator) : base(delegator) { }


        protected override void Run(OnCompleted @return, ITestSuite suite, IExecutionContext context)
        {
            context.Execute(@return, this.RunSuite, suite);
        }


        private void RunSuite(OnCompleted @return, ITestSuite suite)
        {
            IExecutionContext suiteLifetimeContext = this.BuildSuiteContext(suite.BeforeAll, suite.AfterAll);
            suiteLifetimeContext.Execute(@return, this.RunSuiteContents, suite);
        }


        private void RunSuiteContents(OnCompleted @return, ITestSuite suite)
        {
            IExecutionContext descendantsContext = this.BuildSuiteContext(suite.BeforeEach, suite.AfterEach);
            base.Run(@return, suite, descendantsContext);
        }


        private IExecutionContext BuildSuiteContext(IAction before, IAction after)
        {
            return before is null && after is null
                ? (IExecutionContext) new EmptyExecutionContext()
                : new BeforeAfterContext(this.CallbackFor(before), this.CallbackFor(after));
        }


        private Action<Action> CallbackFor(IAction action)
        {
            return action is null
                ? (Action<Action>) (callback => callback())
                : callback => this.Delegator.Run(callback, action.Action, action.Delay);
        }


        private class BeforeAfterContext : IExecutionContext
        {
            private readonly Action<Action> Before;
            private readonly Action<Action> After;

            public BeforeAfterContext(Action<Action> before, Action<Action> after)
            {
                this.Before = before;
                this.After = after;
            }

            public void Execute<T>(OnCompleted @return, Action<OnCompleted, T> executable, T node)
                where T : ITraversable
            {
                this.Before(() => executable.Invoke(result => this.After(() => @return(result)), node));
            }
        }
    }
}
