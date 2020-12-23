using System;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Builders
{
    public interface ITestableNodeBuilder<in TMethod, out TBuilt, out TThis> : INodeBuilder<TBuilt, TThis>
        where TMethod : Delegate
        where TBuilt : ITestable, INode
        where TThis : ITestableNodeBuilder<TMethod, TBuilt, TThis>
    {
        public TThis SetTestMethod(TMethod testMethod);
    }
}