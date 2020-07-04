using System;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework
{
    internal interface ITest
    {
        public string Name { get; }
        public void RunTest();
        public ILogger GetResults();
    }

    internal static class TestExtensions
    {
        public static ITest Guard(this ITest test, Func<TestResult> guardMethod)
        {
            return new GuardedTest(test, guardMethod);
        }
    }
}
