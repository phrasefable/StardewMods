using System.Collections.Generic;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model
{
    public interface ICasedTest : IBaseTest
    {
        public IEnumerable<object> Cases { get; }
    }

    public interface ICasedTest<out TCaseParams> : ICasedTest
    {
        public new IEnumerable<TCaseParams> Cases { get; }
    }
}