using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Internal.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Runners
{
    internal class TestRunner : TraversableRunner<ITest>
    {
        protected override void Run(OnCompleted @return, ITest test, IExecutionContext context)
        {
            context.Execute(@return, TestRunner.RunTest, test);
        }


        protected override void Skip(OnCompleted @return, ITest test, Status status, string message)
        {
            @return(new TestResult(test) { Status = status, Message = message });
        }


        private static void RunTest(OnCompleted @return, ITest test)
        {
            @return(new TestResult(test, test.TestMethod.Invoke()));
        }
    }
}