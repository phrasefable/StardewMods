using System;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Loggers;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework
{
    internal class Test : ITest
    {
        private readonly Func<TestResult> _testMethod;
        internal TestResult Result { get; private set; }

        public string Name { get; }


        public Test(string name, Func<TestResult> testMethod)
        {
            this._testMethod = testMethod;
            this.Name = name;
        }


        public void RunTest()
        {
            this.Result = this._testMethod();
        }


        public ILogger GetResults()
        {
            var logger = new ResultLogger(this) {HasFailure = this.Result.Outcome != TestOutcome.Pass};

            logger.Append(this.Result);

            return logger;
        }
    }
}
