using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Framework.Builders
{
    public interface ITestBuilder : IBuilder<ITest>, ITraversableBuilder
    {
        public Func<ITestResult> TestMethod { set; }
    }
}
