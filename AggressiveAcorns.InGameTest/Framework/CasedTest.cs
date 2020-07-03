using System;
using System.Collections.Generic;
using System.Linq;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework
{
    internal class CasedTest<TIn, TOut> : ITest
    {
        // ==================================== Internal Types ====================================

        private readonly struct Case
        {
            public readonly TIn Input;
            public readonly TOut ExpectedOutput;

            public Case(TIn input, TOut expectedOutput)
            {
                this.Input = input;
                this.ExpectedOutput = expectedOutput;
            }
        }

        private class CaseResult : TestResult
        {
            public Case Case { get; }

            public CaseResult(Case @case, TestResult result)
                : base(result.Outcome, result.Message)
            {
                this.Case = @case;
            }
        }

        public delegate TestResult TestMethod(TIn testParameters, TOut expectedOutput);

        // ========================================================================================

        private readonly TestMethod _testMethod;
        private readonly List<Case> _cases = new List<Case>();

        private readonly IDictionary<TestOutcome, List<CaseResult>> _resultsByOutcome =
            new Dictionary<TestOutcome, List<CaseResult>>();

        public string Name { get; }

        public Func<TIn, string> DescribeCase { set; private get; } = param => param.ToString();


        public CasedTest(string name, TestMethod testMethod)
        {
            this._testMethod = testMethod;
            this.Name = name;
        }


        public void AddCase(TIn caseArgs, TOut expectedOutcome)
        {
            this._cases.Add(new Case(caseArgs, expectedOutcome));
        }


        public void RunTest()
        {
            foreach (TestOutcome outcome in TestOutcomeExtensions.TestOutcomes())
            {
                this._resultsByOutcome[outcome] = new List<CaseResult>();
            }

            foreach (Case @case in this._cases)
            {
                TestResult result = this._testMethod(@case.Input, @case.ExpectedOutput);
                this._resultsByOutcome[result.Outcome].Add(new CaseResult(@case, result));
            }
        }


        public ResultLogger GetResults()
        {
            var logger = new ResultLogger(this);

            void ListBadCases(TestOutcome outcome)
            {
                logger.In.Append(outcome.Name() + ":");

                int longest = this._resultsByOutcome[outcome]
                    .Max(result => this.DescribeCase(result.Case.Input).Length);

                foreach (CaseResult result in this._resultsByOutcome[outcome])
                {
                    string @string = this.DescribeCase(result.Case.Input);
                    if (!string.IsNullOrEmpty(result.Message))
                    {
                        @string = @string.PadRight(longest) + " - " + result.Message;
                    }

                    logger.In.In.Append(@string);
                }
            }

            logger.HasFailure = this._resultsByOutcome.Any(
                pair => pair.Value.Any() && pair.Key != TestOutcome.Pass
            );

            logger.Append(this.GetTally());

            foreach (TestOutcome outcome in new[] {TestOutcome.Fail, TestOutcome.NotRun})
            {
                if (this._resultsByOutcome[outcome].Count > 0) ListBadCases(outcome);
            }

            return logger;
        }


        private string GetTally()
        {
            return string.Join(
                "  |  ",
                TestOutcomeExtensions.TestOutcomes()
                    .Select(outcome => $"{outcome.Name()}: {this._resultsByOutcome[outcome].Count,3}")
            );
        }
    }
}
