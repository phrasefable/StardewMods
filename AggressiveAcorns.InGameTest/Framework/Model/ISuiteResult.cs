using System.Collections.Generic;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model
{
    public interface ISuiteResult : IIdentifiable, IResult
    {
        IEnumerable<TypeUnion<ISuiteResult, IResult>> Children { get; }
    }
}