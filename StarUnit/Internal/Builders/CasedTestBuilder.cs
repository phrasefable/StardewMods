using System;
using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Model;
using Phrasefable.StardewMods.StarUnit.Internal.Model;

namespace Phrasefable.StardewMods.StarUnit.Internal.Builders
{
    internal class CasedTestBuilder<TCaseParams> : ICasedTestBuilder<TCaseParams>
    {
        private readonly TestGrouping _grouping;

        private readonly IdentifiableBuilder _identifiableBuilder;
        private readonly IList<Func<IResult>> _conditions;
        private readonly SettableOnce<Func<TCaseParams, IResult>> _testMethod;

        private readonly IList<TCaseParams> _cases;

        private readonly SettableOnce<Func<TCaseParams, string>> _keyGenerator;

        private readonly SettableOnce<Func<TCaseParams, string>> _longNameGenerator;

        private readonly Func<ITestBuilder> _testBuilderGenerator;


        public CasedTestBuilder(Func<ITestBuilder> testBuilderGenerator)
        {
            this._grouping = new TestGrouping();

            this._identifiableBuilder = new IdentifiableBuilder(this._grouping);

            this._conditions = new List<Func<IResult>>();

            this._testMethod = new SettableOnce<Func<TCaseParams, IResult>>(
                nameof(CasedTestBuilder<TCaseParams>.TestMethod)
            );

            this._cases = new List<TCaseParams>();

            this._keyGenerator = new SettableOnce<Func<TCaseParams, string>>(
                nameof(CasedTestBuilder<TCaseParams>.KeyGenerator)
            );
            this._longNameGenerator = new SettableOnce<Func<TCaseParams, string>>(
                nameof(CasedTestBuilder<TCaseParams>.LongNameGenerator)
            );

            this._testBuilderGenerator = testBuilderGenerator;
        }

        public ITestGrouping Build()
        {
            this._identifiableBuilder.Build();

            this._grouping.Conditions = this._conditions;

            var i = 1;
            if (!this._keyGenerator.HasBeenSet)
            {
                KeyGenerator = @case => this._grouping.Key + i++;
            }

            var branchBuilder = new BranchChildrenBuilder<ITest>();
            foreach (TCaseParams @case in this._cases)
            {
                branchBuilder.AddChild(this.BuildCase(@case));
            }

            this._grouping.Children = branchBuilder.Build();


            return this._grouping;
        }

        private ITest BuildCase(TCaseParams @case)
        {
            ITestBuilder builder = this._testBuilderGenerator();

            builder.Key = this._keyGenerator.Value(@case);
            if (this._longNameGenerator.HasBeenSet) builder.LongName = this._longNameGenerator.Value(@case);
            Func<TCaseParams, IResult> testMethod = this._testMethod.Value;
            builder.TestMethod = () => testMethod(@case);

            return builder.Build();
        }

        public string Key
        {
            set => this._identifiableBuilder.Key = value;
        }

        public string LongName
        {
            set => this._identifiableBuilder.Key = value;
        }

        public void AddCondition(Func<IResult> condition)
        {
            this._conditions.Add(condition);
        }

        public Func<TCaseParams, IResult> TestMethod
        {
            set => this._testMethod.Value = value;
        }

        // be sure that if the cases are the same that they will still get unique keys.
        public void AddCases(params TCaseParams[] cases)
        {
            foreach (TCaseParams @case in cases)
            {
                this._cases.Add(@case);
            }
        }

        public Func<TCaseParams, string> KeyGenerator
        {
            set => this._keyGenerator.Value = value;
        }

        public Func<TCaseParams, string> LongNameGenerator
        {
            set => this._longNameGenerator.Value = value;
        }
    }
}
