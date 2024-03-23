using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using Phrasefable.StardewMods.StarUnit.Internal.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Internal.Builders
{
    internal class TestBuilder : ITestBuilder
    {
        private readonly TraversableBuilder _traversableBuilder;
        private readonly SettableOnce<Func<ITestResult>> _testMethod;


        public TestBuilder()
        {
            this._traversableBuilder = new TraversableBuilder();
            this._testMethod = new SettableOnce<Func<ITestResult>>(nameof(TestBuilder.TestMethod));
        }


        public ITest Build()
        {
            var test = new Test();
            this._traversableBuilder.Build(test);

            if (!this._testMethod.HasBeenSet)
            {
                throw new InvalidOperationException($"{nameof(this.TestMethod)} must be set before building.");
            }

            test.TestMethod = this._testMethod.Value;

            return test;
        }


        public string Key
        {
            set => this._traversableBuilder.Key = value;
        }


        public string LongName
        {
            set => this._traversableBuilder.LongName = value;
        }


        public void AddCondition(Func<IResult> condition)
        {
            this._traversableBuilder.AddCondition(condition);
        }


        public Delay Delay
        {
            set => this._traversableBuilder.Delay = value;
        }


        public Func<ITestResult> TestMethod
        {
            set => this._testMethod.Value = value;
        }
    }
}
