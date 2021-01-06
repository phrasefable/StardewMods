using JetBrains.Annotations;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Model;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using Phrasefable.StardewMods.StarUnit.Internal;
using Phrasefable.StardewMods.StarUnit.Internal.Builders;
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

            helper.ConsoleCommands.Add("list_tests", "Lists test fixtures.", ListTests);
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
            void ConsoleWriter(string s) => this.Monitor.Log(s, LogLevel.Info);

            if (arg2.Length == 0)
            {
                lister = new ConciseLister(ConsoleWriter);
            }
            else if (arg2.Length == 1 && arg2[0] == "-v")
            {
                lister = new VerboseLister(ConsoleWriter);
            }
            else
            {
                this.Monitor.Log("Invalid arguments.", LogLevel.Error);
                return;
            }

            Monitor.Log("Registered tests:", LogLevel.Info);
            Monitor.Log("", LogLevel.Info);

            foreach (ITestSuite suite in this._tests.TestRoots)
            {
                lister.List(suite);
            }
        }
    }
}
