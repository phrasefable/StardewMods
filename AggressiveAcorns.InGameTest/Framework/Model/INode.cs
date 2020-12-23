using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model
{
    public interface INode
    {
        [NotNull] public string Key { get; }
        [CanBeNull] public string Name { get; }

        [CanBeNull] public INode Parent { get; }

        [NotNull] public IEnumerable<ITestable> Conditions { get; }

        [CanBeNull] public Action BeforeAll { get; }
        [CanBeNull] public Action BeforeEach { get; }
        [CanBeNull] public Action AfterEach { get; }
        [CanBeNull] public Action AfterAll { get; }
    }
}
