using System;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Builders
{
    public class BuilderFactory : IBuilderFactory
    {
        private readonly IValidator _validator;


        public BuilderFactory(IValidator validator)
        {
            this._validator = validator;
        }


        public ICasedTestBuilder<TCaseParams> CreateCasedTestBuilder<TCaseParams>()
        {
            return new CasedTestBuilder<TCaseParams>(this._validator);
        }


        public ITestFixtureBuilder CreateFixtureBuilder()
        {
            return new TestFixtureBuilder(this._validator);
        }


        public ITestBuilder CreateTestBuilder()
        {
            throw new NotImplementedException();
        }
    }
}