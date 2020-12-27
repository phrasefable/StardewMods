using System;
using System.Collections.Generic;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Builders
{
    public class CasedTestBuilder<TCaseParams> : ICasedTestBuilder<TCaseParams>
    {
        private readonly CasedTest<TCaseParams> _test;

        private readonly IList<TCaseParams> _cases = new List<TCaseParams>();
        private readonly IList<Func<IResult>> _conditions = new List<Func<IResult>>();

        private readonly IValidator _validator;


        public CasedTestBuilder(IValidator validator)
        {
            this._test = new CasedTest<TCaseParams>
            {
                Cases = this._cases,
                Conditions = this._conditions
            };

            this._validator = validator;
        }


        public ICasedTest<TCaseParams> Build()
        {
            this._validator.Validate<IIdentifiable>(this._test);
            this._validator.Validate<IConditional>(this._test);

            return this._test;
        }


        public void SetKey(string value)
        {
            this._test.Key = value;
        }


        public void SetLongName(string value)
        {
            this._test.LongName = value;
        }


        public void SetTestMethod(Func<TCaseParams, IResult> testMethod)
        {
            this._test.TestMethod = testMethod;
        }


        public void AddCases(params TCaseParams[] cases)
        {
            foreach (TCaseParams @case in cases)
            {
                this._cases.Add(@case);
            }
        }


        public void AddCondition(Func<IResult> condition)
        {
            this._conditions.Add(condition);
        }
    }
}