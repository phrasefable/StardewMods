using System;
using System.Collections.Generic;
using System.Linq;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model
{
    public class CasedTest<TCaseParams> : ICasedTest<TCaseParams>
    {
        public string Key { get; set; }
        public string LongName { get; set; }

        public IEnumerable<TCaseParams> Cases { get; set; }
        public IEnumerable<Func<IResult>> Conditions { get; set; }

        public Func<TCaseParams, IResult> TestMethod { get; set; }

        IEnumerable<object> ICasedTest.Cases => this.Cases.Cast<object>();
    }
}