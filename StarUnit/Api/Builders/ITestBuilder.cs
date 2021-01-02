using System;
using Phrasefable.StardewMods.StarUnit.Api.Model;

namespace Phrasefable.StardewMods.StarUnit.Api.Builders
{
    public interface ITestBuilder : IBuilder<ITest>, ITraversableBuilder
    {
        public Func<Result> TestMethod { set; }
    }
}