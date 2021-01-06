using System;

namespace Phrasefable.StardewMods.StarUnit.Framework.Builders
{
    public interface IConditionalBuilder
    {
        public void AddCondition(Func<IResult> condition);
    }
}