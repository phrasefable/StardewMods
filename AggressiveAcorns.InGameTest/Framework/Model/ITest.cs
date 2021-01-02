using System;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model
{
    public interface ITest : IRunnable, IIdentifiable { }

    class Test : ITest
    {
        public string Key { get; set; }
        public string LongName { get; set; }

        public Func<IResult> TestMethod { get; set; }
    }
}