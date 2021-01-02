using System;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Builders
{
    public interface ITestBuilder : IBuilder<ITest>, IIdentifiableBuilder
    {
        public void SetTestMethod(Func<IResult> testMethod);
    }

    public class TestBuilder : ITestBuilder
    {
        private readonly Test _test = new Test();

        private readonly IValidator _validator;


        public TestBuilder(IValidator validator)
        {
            this._validator = validator;
        }


        public ITest Build()
        {
            this._validator.Validate<IIdentifiable>(this._test);
            return this._test;
        }


        public void SetTestMethod(Func<IResult> testMethod)
        {
            this._test.TestMethod = testMethod;
        }


        public void SetKey(string key)
        {
            this._test.Key = key;
        }


        public void SetLongName(string longName)
        {
            this._test.LongName = longName;
        }
    }
}