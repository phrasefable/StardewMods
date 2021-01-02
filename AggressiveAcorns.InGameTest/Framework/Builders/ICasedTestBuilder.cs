using System;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Builders
{
    public interface ICasedTestBuilder<TCaseParams> : IIdentifiableBuilder, IBuilder<ITestSuite>
    {
        public void AddCases(params TCaseParams[] @case);
        public void SetTestMethod(Func<TCaseParams, IResult> testMethod);
    }
}