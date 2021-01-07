using System;
using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Builders
{
    internal class CasedTestBuilder<TCaseParams> : ICasedTestBuilder<TCaseParams>
    {
        private readonly ITestFixtureBuilder _fixtureBuilder;

        private readonly SettableOnce<Func<TCaseParams, ITestResult>> _testMethod;

        private readonly IList<TCaseParams> _cases;

        private readonly SettableOnce<Func<TCaseParams, string>> _keyGenerator;

        private readonly SettableOnce<Func<TCaseParams, string>> _longNameGenerator;

        private readonly Func<ITestBuilder> _testBuilderFactory;

        private string _key;


        public CasedTestBuilder(ITestDefinitionFactory factory)
        {
            this._fixtureBuilder = factory.CreateFixtureBuilder();

            this._testMethod = new SettableOnce<Func<TCaseParams, ITestResult>>(
                nameof(CasedTestBuilder<TCaseParams>.TestMethod)
            );

            this._cases = new List<TCaseParams>();

            this._keyGenerator = new SettableOnce<Func<TCaseParams, string>>(
                nameof(CasedTestBuilder<TCaseParams>.KeyGenerator)
            );
            this._longNameGenerator = new SettableOnce<Func<TCaseParams, string>>(
                nameof(CasedTestBuilder<TCaseParams>.LongNameGenerator)
            );

            this._testBuilderFactory = factory.CreateTestBuilder;
        }


        public ITestSuite Build()
        {
            var i = 1;
            if (!this._keyGenerator.HasBeenSet)
            {
                this.KeyGenerator = @case => this._key + i++;
            }

            var branchBuilder = new BranchChildrenBuilder<ITest>();
            foreach (TCaseParams @case in this._cases)
            {
                branchBuilder.AddChild(this.BuildCase(@case));
            }

            foreach (ITest test in branchBuilder.Build())
            {
                this._fixtureBuilder.AddChild(test);
            }

            return this._fixtureBuilder.Build();
        }


        private ITest BuildCase(TCaseParams @case)
        {
            ITestBuilder builder = this._testBuilderFactory();

            builder.Key = this._keyGenerator.Value(@case);
            if (this._longNameGenerator.HasBeenSet) builder.LongName = this._longNameGenerator.Value(@case);
            Func<TCaseParams, ITestResult> testMethod = this._testMethod.Value;
            builder.TestMethod = () => testMethod(@case);

            return builder.Build();
        }


        public string Key
        {
            set
            {
                this._fixtureBuilder.Key = value;
                this._key = value;
            }
        }


        public string LongName
        {
            set => this._fixtureBuilder.Key = value;
        }


        public void AddCondition(Func<IResult> condition)
        {
            this._fixtureBuilder.AddCondition(condition);
        }


        public Func<TCaseParams, ITestResult> TestMethod
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