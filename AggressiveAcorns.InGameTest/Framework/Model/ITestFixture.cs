using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model
{
    public interface ITestFixture : IIdentifiable, IConditional
    {
        [NotNull] public IEnumerable<IBaseTest> Tests { get; }

        [CanBeNull] public Action BeforeAll { get; }
        [CanBeNull] public Action BeforeEach { get; }
        [CanBeNull] public Action AfterEach { get; }
        [CanBeNull] public Action AfterAll { get; }
    }
}