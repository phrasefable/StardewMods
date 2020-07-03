using System;
using System.Collections.Generic;
using System.Linq;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework
{
    internal enum TestOutcome
    {
        Pass,
        Fail,
        NotRun
    }

    internal static class TestOutcomeExtensions
    {
        public static string Name(this TestOutcome outcome)
        {
            return outcome switch
            {
                TestOutcome.Pass => "Passed",
                TestOutcome.Fail => "Failed",
                TestOutcome.NotRun => "Skipped",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static IEnumerable<TestOutcome> TestOutcomes()
        {
            return Enum.GetValues(typeof(TestOutcome)).Cast<TestOutcome>();
        }
    }
}