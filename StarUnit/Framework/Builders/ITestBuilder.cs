using System;
using Phrasefable.StardewMods.StarUnit.Framework.Model;

namespace Phrasefable.StardewMods.StarUnit.Framework.Builders
{
    public interface ITestBuilder : IBuilder<ITest>, ITraversableBuilder
    {
        public Func<Result> TestMethod { set; }
    }
}