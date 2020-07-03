using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Tests;
using StardewModdingAPI;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest
{
    [UsedImplicitly]
    public class ModEntry : Mod
    {
        private readonly Dictionary<string, ITest> _tests = new Dictionary<string, ITest>();
        private string TestNames => string.Join(" ", _tests.Keys);


        public override void Entry(IModHelper helper)
        {
            this._tests.Add("experiences_winter", TreeUtils_ExperiencesWinter_Test.BuildTest());

            var desc = new StringBuilder();
            desc.AppendLine("");
            desc.AppendLine("Usage:");
            desc.AppendLine("  run_tests [<test> ...]");
            desc.AppendLine("Runs specified tests. If none specified, runs all tests.");
            desc.Append("Use command `list_tests` to determining test names.");

            helper.ConsoleCommands.Add("run_tests", desc.ToString(), this.RunTests);
            helper.ConsoleCommands.Add("list_tests", "Lists registered tests.", ListTests);
        }


        private void ListTests(string arg1, string[] arg2)
        {
            Monitor.Log(this.TestNames);
        }


        private void RunTests(string command, string[] args)
        {
            void RunTest(ITest test)
            {
                test.RunTest();
                test.GetResults().Log(Monitor);
            }

            if (args.Length == 0)
            {
                foreach (ITest test in this._tests.Values)
                {
                    RunTest(test);
                }
            }

            List<string> badKeys = args
                .Where(arg => !this._tests.ContainsKey(arg))
                .ToList();

            if (badKeys.Any())
            {
                Monitor.Log(
                    $"Test run aborted. Invalid test name(s) present: {string.Join(" ", badKeys)}",
                    LogLevel.Error
                );
                Monitor.Log($"Available tests: {this.TestNames}", LogLevel.Error);
                return;
            }

            foreach (string testId in args)
            {
                RunTest(this._tests[testId]);
            }
        }
    }
}
