using System;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using Phrasefable.StardewMods.StarUnit.Internal.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Runners
{
    internal class TestSuiteRunner : TraversableRunner<ITestSuite>
    {
        private readonly IRunner ChildRunner;


        public TestSuiteRunner(IRunner childRunner)
        {
            this.ChildRunner = childRunner;
        }


        protected override ITraversableResult _Run(ITestSuite suite)
        {
            suite.BeforeAll?.Invoke();
            ITraversableResult testResult = TestSuiteRunner.ActOnChildren(
                Status.Pass,
                suite,
                child => this.RunChild(suite, child)
            );
            suite.AfterAll?.Invoke();
            return testResult;
        }


        protected override ITraversableResult _Skip(ITestSuite node)
        {
            return TestSuiteRunner.ActOnChildren(
                Status.Skipped,
                node,
                child => this.ChildRunner.Skip(child)
            );
        }


        private ITraversableResult RunChild(ITestSuite suite, ITraversable child)
        {
            suite.BeforeEach?.Invoke();
            ITraversableResult childTestResult = this.ChildRunner.Run(child);
            suite.AfterEach?.Invoke();
            return childTestResult;
        }


        private static ITraversableResult ActOnChildren(
            Status status,
            ITestSuite suite,
            Func<ITraversable, ITraversableResult> action)
        {
            var result = new SuiteResult(suite) {Status = status};

            foreach (ITraversable child in suite.Children)
            {
                TestSuiteRunner.ActOnChild(action, child, result);
            }

            return result;
        }


        private static void ActOnChild(
            Func<ITraversable, ITraversableResult> action,
            ITraversable child,
            SuiteResult result)
        {
            ITraversableResult childTestResult = action(child);

            result.Children.Add(childTestResult);

            if (childTestResult is ISuiteResult suiteResult)
            {
                result.DescendantLeafTallies.AddToValues(suiteResult.DescendantLeafTallies);
                result.TotalDescendantLeaves += suiteResult.TotalDescendantLeaves;
            }
            else
            {
                result.DescendantLeafTallies.AddToValue(childTestResult.Status, 1);
                result.TotalDescendantLeaves += 1;
            }
        }
    }
}
