using System;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Builders
{
    public interface IBaseTestBuilder<out TThis, in TMethod> : IIdentifiableBuilder, IConditionalBuilder
        where TThis : IBaseTestBuilder<TThis, TMethod>
        where TMethod : Delegate
    {
        public void SetTestMethod(TMethod testMethod);
    }
}