using JetBrains.Annotations;
using Phrasefable.StardewMods.StarUnit.Internal;
using Phrasefable.StardewMods.StarUnit.Internal.Builders;
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
        }


        public override object GetApi()
        {
            return new StarUnitApi(this._tests.Register)
            {
                BuilderFactory = new BuilderFactory()
            };
        }
    }
}
