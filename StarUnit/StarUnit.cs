using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using Phrasefable.StardewMods.StarUnit.Internal;
using Phrasefable.StardewMods.StarUnit.Internal.Builders;
using Phrasefable.StardewMods.StarUnit.Internal.Filterers;
using Phrasefable.StardewMods.StarUnit.Internal.Filterers.Wrappers;
using Phrasefable.StardewMods.StarUnit.Internal.ResultListers;
using Phrasefable.StardewMods.StarUnit.Internal.Runners;
using Phrasefable.StardewMods.StarUnit.Internal.SmapiAdaptors;
using Phrasefable.StardewMods.StarUnit.Internal.TestListers;
using StardewModdingAPI;

namespace Phrasefable.StardewMods.StarUnit
{
    // TODO collapse non-branching runs, even when folded.
    [UsedImplicitly]
    public class StarUnit : Mod
    {
        private TestRegistry _tests;

        private readonly string _cmdListTests = "list_tests";
        private readonly string _cmdRunTests = "run_tests";
        private readonly string _argVerbose = "-v";


        public override void Entry(IModHelper helper)
        {
            this._tests = new TestRegistry(
                // ReSharper disable once RedundantArgumentDefaultValue
                s => this.Monitor.Log(s, LogLevel.Trace),
                s => this.Monitor.Log(s, LogLevel.Error)
            );

            helper.ConsoleCommands.Add(this._cmdListTests, "Lists test fixtures.", this.ListTests);
            helper.ConsoleCommands.Add(this._cmdRunTests, "Runs test fixtures.", this.RunTests);
        }


        public override object GetApi()
        {
            return new StarUnitApi(this._tests.Register)
            {
                TestDefinitionFactory = new TestDefinitionFactory()
            };
        }


        private void ListTests(string arg1, string[] arg2)
        {
            // TODO: make conditions show with explanations?

            void ConsoleWriter(string s) => this.Monitor.Log(s, LogLevel.Info);
            bool IsVerboseArg(string arg) => arg == this._argVerbose;
            bool IsNotVerboseArg(string arg) => arg != this._argVerbose;

            ILister lister;
            string[] filterStrings;

            if (arg2.Length > 0 && arg2.Any(IsVerboseArg))
            {
                lister = new VerboseLister(ConsoleWriter);
                filterStrings = arg2.Where(IsNotVerboseArg).ToArray();
            }
            else
            {
                lister = new ConciseLister(ConsoleWriter);
                filterStrings = arg2;
            }

            this.Monitor.Log("Registered tests:", LogLevel.Info);
            this.Monitor.Log("", LogLevel.Info);

            foreach (ITraversable root in this.GetFilteredTestsRoots(filterStrings))
            {
                lister.List(root);
            }
        }


        private void RunTests(string arg1, string[] arg2)
        {
            ITraversable[] testsToRun = GetFilteredTestsRoots(arg2).ToArray();

            IResultLister lister = this.BuildResultLister();
            ITreeRunner runner = this.BuildTestRunner();

            ICollection<ITraversableResult> results = new List<ITraversableResult>(testsToRun.Length);

            testsToRun.ForEach(() => lister.List(results), runner.Run, results.Add);
        }


        private IEnumerable<ITraversable> GetFilteredTestsRoots(string[] filterStrings)
        {
            if (filterStrings is null || !filterStrings.Any()) return this._tests.TestRoots;

            IEnumerable<IStringNode> filterTrees = new FilterParser().BuildFilterTrees(filterStrings);
            IFilterer filterer = StarUnit.BuildFilterer();
            return this._tests.TestRoots.Select(root => filterer.Filter(root, filterTrees));
        }


        private ITreeRunner BuildTestRunner()
        {
            var runner = new Runner(new SmapiTicker(this.Helper.Events.GameLoop));
            runner.Add(new TestRunner());
            runner.Add(new GroupingRunner(runner));
            runner.Add(new TestSuiteRunner(runner));
            return runner;
        }


        private IResultLister BuildResultLister()
        {
            var lister = new CompositeResultLister<ResultListingContext>();
            lister.Add(new TestResultLister(this.WriteToConsole));
            lister.Add(new BranchResultLister(this.WriteToConsole, lister));
            return lister;
        }


        private static IFilterer BuildFilterer()
        {
            var filterer = new CompositeFilterer();
            filterer.Add(new BranchFilterer<ITestSuite>(filterer, new TestSuiteWrapperFactory()));
            filterer.Add(new BranchFilterer<ITraversableGrouping>(filterer, new TraversableGroupingWrapperFactory()));
            filterer.Add(new TestFilterer());
            return filterer;
        }


        private void WriteToConsole(string message, Status status)
        {
            this.Monitor.Log(
                message,
                status switch
                {
                    Status.Pass => LogLevel.Info,
                    Status.Fail => LogLevel.Warn,
                    Status.Error => LogLevel.Warn,
                    Status.Skipped => LogLevel.Warn,
                    _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
                }
            );
        }
    }
}
