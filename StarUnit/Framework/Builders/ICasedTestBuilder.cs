using System;
using Phrasefable.StardewMods.StarUnit.Framework.Model;

namespace Phrasefable.StardewMods.StarUnit.Framework.Builders
{
    public interface ICasedTestBuilder<TCaseParams> : IBuilder<ITestSuite>, ITraversableBuilder
    {
        public Func<TCaseParams, IResult> TestMethod { set; }

        public void AddCases(params TCaseParams[] cases);

        public Func<TCaseParams, string> KeyGenerator { set; }
        public Func<TCaseParams, string> LongNameGenerator { set; }
    }
}
