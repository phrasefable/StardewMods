using System;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Builders
{
    public interface ICasedTestBuilder<TCaseParams> :
        IBaseTestBuilder<ICasedTestBuilder<TCaseParams>, Func<TCaseParams, IResult>>,
        IBuilder<ICasedTest<TCaseParams>>
    {
        public void AddCases(params TCaseParams[] @case);
    }
}