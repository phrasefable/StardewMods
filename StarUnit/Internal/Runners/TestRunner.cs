using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using Phrasefable.StardewMods.StarUnit.Internal.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Runners
{
    internal class TestRunner : ComponentRunner<ITest>
    {
        protected override ITraversableResult _Run(ITest test)
        {
            var result = new TestResult(test);
            IResult rawResult = test.TestMethod.Invoke();
            result.Status = rawResult.Status;
            result.Message = rawResult.Message;
            return result;
        }


        protected override ITraversableResult _Run(ITest test, IExecutionContext context)
        {
            return context.Execute(test, this.Run);
        }


        protected override ITraversableResult _Skip(ITest test)
        {
            return this._Skip(test, Status.Skipped, null);
        }


        protected override ITraversableResult _Skip(ITest test, Status status, string message)
        {
            return new TestResult(test) {Status = status, Message = message};
        }
    }
}
