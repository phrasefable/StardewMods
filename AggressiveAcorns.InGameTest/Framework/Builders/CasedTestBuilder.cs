using System;
using System.Collections.Generic;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Builders
{
    public class CasedTestBuilder<TCaseParams> : ICasedTestBuilder<TCaseParams>
    {
        private readonly ITestFixtureBuilder _caseSuite;

        private Func<TCaseParams, IResult> _testMethod;
        private readonly IList<TCaseParams> _cases = new List<TCaseParams>();


        private readonly IBuilderFactory _builderFactory;
        // private readonly IValidator _validator;

        private int _caseId = 0;
        public Func<TCaseParams, string> KeyGenerator { get; set; }
        public Func<TCaseParams, string> LongNameGenerator { get; set; } = null;


        public CasedTestBuilder(IValidator validator, IBuilderFactory builderFactory)
        {
            this._caseSuite = builderFactory.CreateFixtureBuilder();
            this.KeyGenerator = caseParams => (_caseId++).ToString();

            this._builderFactory = builderFactory;
        }


        private ITest BuildCase(TCaseParams caseParams)
        {
            ITestBuilder testBuilder = this._builderFactory.CreateTestBuilder();

            testBuilder.SetKey(this.KeyGenerator(caseParams));
            if (this.LongNameGenerator != null) testBuilder.SetLongName(this.LongNameGenerator(caseParams));
            testBuilder.SetTestMethod(() => this._testMethod(caseParams));

            return testBuilder.Build();
        }


        public ITestSuite Build()
        {
            foreach (TCaseParams @case in this._cases)
            {
                this._caseSuite.AddChild(this.BuildCase(@case));
            }

            return this._caseSuite.Build();
        }


        public void SetKey(string value)
        {
            this._caseSuite.SetKey(value);
        }


        public void SetLongName(string value)
        {
            this._caseSuite.SetLongName(value);
        }


        // public void AddCondition(Func<IResult> condition)
        // {
        //     this._caseSuite.AddCondition(condition);
        // }


        public void SetTestMethod(Func<TCaseParams, IResult> testMethod)
        {
            this._testMethod = testMethod;
        }


        public void AddCases(params TCaseParams[] cases)
        {
            foreach (TCaseParams @case in cases)
            {
                this._cases.Add(@case);
            }
        }
    }
}
