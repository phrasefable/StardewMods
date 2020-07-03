using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework;
using StardewModdingAPI;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest
{
    [UsedImplicitly]
    public class ModEntry : Mod
    {
        private readonly Dictionary<string, ITest> _tests = new Dictionary<string, ITest>();


        public override void Entry(IModHelper helper)
        {
            var desc = new StringBuilder("Usage: run_tests [<test> ...]");
            desc.AppendLine();
            desc.AppendLine("Runs specified tests. If none specified, runs all tests.");
            desc.AppendLine("See `list_tests` for determining test names.");

            helper.ConsoleCommands.Add("run_tests", desc.ToString(), this.RunTests);
            helper.ConsoleCommands.Add("list_tests", "Lists registered tests.", ListTests);
        }


        private void ListTests(string arg1, string[] arg2)
        {
            Monitor.Log(string.Join(" ", _tests.Keys));
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
                Monitor.Log($"Invalid test ids: {string.Join(" ", badKeys)}", LogLevel.Error);
                Monitor.Log($"Test ids must be one of: {string.Join(" ", this._tests.Keys)}", LogLevel.Error);
                return;
            }

            foreach (string testId in args)
            {
                RunTest(this._tests[testId]);
            }
        }
    }
}
