using System.Collections.Generic;
using JetBrains.Annotations;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model
{
    public interface ITestSuite : INode
    {
        [NotNull] public IEnumerable<INode> Children { get; }
    }
}