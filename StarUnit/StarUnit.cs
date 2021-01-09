using System;
using System.Linq;
using JetBrains.Annotations;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Internal;
using Phrasefable.StardewMods.StarUnit.Internal.Builders;
using Phrasefable.StardewMods.StarUnit.Internal.ResultListers;
using Phrasefable.StardewMods.StarUnit.Internal.Runners;
using Phrasefable.StardewMods.StarUnit.Internal.TestListers;
using StardewModdingAPI;

namespace Phrasefable.StardewMods.StarUnit
{
    [UsedImplicitly]
    public class StarUnit : Mod
    {
        private TestRegistry _tests;


        public override void Entry(IModHelper helper)
        {
            this._tests = new TestRegistry(
                s => this.Monitor.Log(s, LogLevel.Trace),
                s => this.Monitor.Log(s, LogLevel.Error)
            );

            helper.ConsoleCommands.Add("list_tests", "Lists test fixtures.", this.ListTests);
            helper.ConsoleCommands.Add("run_tests", "Runs test fixtures.", this.RunTests);
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
            // TODO: filter via args

            ILister lister;

            void ConsoleWriter(string s)
            {
                this.Monitor.Log(s, LogLevel.Info);
            }

            switch (arg2.Length)
            {
                case 0:
                    lister = new ConciseLister(ConsoleWriter);
                    break;
                case 1 when arg2[0] == "-v":
                    lister = new VerboseLister(ConsoleWriter);
                    break;
                default:
                    this.Monitor.Log("Invalid arguments.", LogLevel.Error);
                    return;
            }

            this.Monitor.Log("Registered tests:", LogLevel.Info);
            this.Monitor.Log("", LogLevel.Info);

            foreach (ITraversable root in this._tests.TestRoots)
            {
                lister.List(root);
            }
        }


        private void RunTests(string arg1, string[] arg2)
        {
            // TODO: filter via args
            IRunner runner = StarUnit.BuildTestRunner();
            IResultLister lister = this.BuildResultLister();

            lister.List(this._tests.TestRoots.Select(suite => runner.Run(suite)));
        }


        private static IRunner BuildTestRunner()
        {
            ICompositeRunner runner = new TraversableRunner();
            runner.Add(new TestRunner());
            runner.Add(new GroupingRunner(runner));
            runner.Add(new TestSuiteRunner(runner));
            return runner;
        }


        private IResultLister BuildResultLister()
        {
            var lister = new CompositeResultLister<ResultListingContext>();
            lister.Add(new TestResultLister(this.WriteToConsole));
            lister.Add(new TestSuiteResultLister(this.WriteToConsole, lister));
            return lister;
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
