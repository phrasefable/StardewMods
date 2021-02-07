using System;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Framework.Builders
{
    public interface ITraversableBuilder
    {
        public string Key { set; }
        public string LongName { set; }

        public void AddCondition(Func<IResult> condition);

        public Delay Delay { set; }
    }
}