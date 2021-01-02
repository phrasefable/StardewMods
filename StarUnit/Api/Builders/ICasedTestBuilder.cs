using System;
using Phrasefable.StardewMods.StarUnit.Api.Model;

namespace Phrasefable.StardewMods.StarUnit.Api.Builders
{
    public interface ICasedTestBuilder<TCaseParams> : IBuilder<ITestGrouping>, ITraversableBuilder
    {
        public Func<TCaseParams, Result> TestMethod { set; }

        public void AddCases(params TCaseParams[] cases);

        public Func<TCaseParams, string> KeyGenerator { set; }
        public Func<TCaseParams, string> LongNameGenerator { set; }
    }
}