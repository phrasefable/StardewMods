using System;
using JetBrains.Annotations;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework
{
    internal class GuardedTest : ITest
    {
        private readonly Test _guardTest;
        private readonly ITest _guardedTest;

        private bool PassedGuard => this._guardTest.Result.Outcome == TestOutcome.Pass;

        public string Name => this._guardedTest.Name;

        public GuardedTest(ITest guardedTest, [NotNull] Func<TestResult> guardMethod)
        {
            this._guardTest = new Test(guardedTest.Name, guardMethod);
            this._guardedTest = guardedTest;
        }


        public void RunTest()
        {
            this._guardTest.RunTest();
            if (PassedGuard) this._guardedTest.RunTest();
        }


        public ResultLogger GetResults()
        {
            if (PassedGuard) return this._guardedTest.GetResults();

            var logger = new ResultLogger(this._guardedTest) {HasFailure = true};

            string @string = $"Test not run, precondition {this._guardTest.Result.Outcome.Name().ToLower()}";
            string message = this._guardTest.Result.Message;
            if (!string.IsNullOrEmpty(message)) @string += ": " + message;
            logger.Append(@string);

            return logger;
        }
    }
}
