using System;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using Phrasefable.StardewMods.StarUnit.Internal.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Internal.Builders
{
    internal class TestBuilder : ITestBuilder
    {
        private readonly Test _test;

        private readonly IdentifiableBuilder _identifiableBuilder;
        private readonly SettableOnce<Func<ITestResult>> _testMethod;


        public TestBuilder()
        {
            this._test = new Test();

            this._identifiableBuilder = new IdentifiableBuilder();
            this._testMethod = new SettableOnce<Func<ITestResult>>(nameof(TestBuilder.TestMethod));
        }


        public ITest Build()
        {
            this._identifiableBuilder.Build(this._test);

            if (!this._testMethod.HasBeenSet)
            {
                throw new InvalidOperationException($"{nameof(this.TestMethod)} must be set before building.");
            }

            this._test.TestMethod = this._testMethod.Value;

            return this._test;
        }


        public string Key
        {
            set => this._identifiableBuilder.Key = value;
        }


        public string LongName
        {
            set => this._identifiableBuilder.LongName = value;
        }


        public void AddCondition(Func<IResult> condition)
        {
            this._test.Conditions.Add(condition);
        }


        public Func<ITestResult> TestMethod
        {
            set => this._testMethod.Value = value;
        }
    }
}