using System;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Framework.Builders
{
    public interface IConditionalBuilder
    {
        public void AddCondition(Func<IResult> condition);
    }
}
