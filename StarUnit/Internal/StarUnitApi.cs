using System;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Model;

namespace Phrasefable.StardewMods.StarUnit.Internal
{
    // Note (2021-01-03): would mark internal but for SMAPI req. that mod API instantiations must be public types
    public class StarUnitApi : IStarUnitApi
    {
        private readonly Action<string, ITraversable[]> _registrationProxy;

        public StarUnitApi(Action<string, ITraversable[]> registrationProxy)
        {
            this._registrationProxy = registrationProxy;
        }

        public IBuilderFactory BuilderFactory { get; set; }

        public void Register(string modId, params ITraversable[] testNodes)
        {
            this._registrationProxy(modId, testNodes);
        }
    }
}