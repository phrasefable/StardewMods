using System;
using Phrasefable.StardewMods.StarUnit.Framework.Model;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Framework.Builders
{
    public interface ITestBuilder : IBuilder<ITest>, ITraversableBuilder
    {
        public Func<ITestResult> TestMethod { set; }
    }
}
