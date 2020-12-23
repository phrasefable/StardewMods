using System;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Builders
{
    public interface ICasedTestBuilder<TCaseParams> : ITestableNodeBuilder<
        Func<TCaseParams, IResult>,
        ICasedTest,
        ICasedTestBuilder<TCaseParams>
    >
    {
        public ICasedTestBuilder<TCaseParams> AddCase(TCaseParams caseParams);
    }
}