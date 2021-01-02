using System;

namespace Phrasefable.StardewMods.StarUnit.Api.Builders
{
    public interface IConditionalBuilder
    {
        public void AddCondition(Func<Result> condition);
    }
}