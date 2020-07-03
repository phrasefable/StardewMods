using JetBrains.Annotations;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework
{
    internal class TestResult
    {
        public TestOutcome Outcome { get; }
        [CanBeNull] public string Message { get; }


        public TestResult(TestOutcome outcome, string message = null)
        {
            this.Outcome = outcome;
            this.Message = message;
        }


        public override string ToString()
        {
            string s = Outcome.Name();
            if (Message != null) s += ": " + this.Message;
            return s;
        }
    }
}
