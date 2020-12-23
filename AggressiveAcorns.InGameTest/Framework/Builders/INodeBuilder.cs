using System;
using JetBrains.Annotations;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Builders
{
    public interface INodeBuilder<out T, out TThis>
        where T : INode
        where TThis : INodeBuilder<T, TThis>
    {
        [NotNull] public T Build();

        public TThis SetName(string name);

        public TThis AddCondition(ITestable condition);
        public TThis AddCondition(Func<IResult> condition);

        public TThis SetBeforeAllAction(Action action);
        public TThis SetBeforeEachAction(Action action);
        public TThis SetAfterEachAction(Action action);
        public TThis SetAfterAllAction(Action action);
    }
}
