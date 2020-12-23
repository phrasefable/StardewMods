using System;
using StardewModdingAPI;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework
{
    internal static class TestExtensions
    {
        public static ITest Guard(this ITest test, Func<TestResult> guardMethod)
        {
            return new GuardedTest(test, guardMethod);
        }

        public static ITest Guard_WorldReady(this ITest test)
        {
            return test.Guard(
                () => Context.IsWorldReady
                    ? new TestResult(TestOutcome.Pass)
                    : new TestResult(TestOutcome.Fail, "World not ready.")
            );
        }
    }
}
