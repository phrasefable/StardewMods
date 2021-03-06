using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Tests;
using Phrasefable.StardewMods.StarUnit;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest
{
    [UsedImplicitly]
    public class ModEntry : Mod
    {
        private ITestDefinitionFactory _factory;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += OnGameLoopOnGameLaunched;
        }

        private void OnGameLoopOnGameLaunched(object sender, GameLaunchedEventArgs args)
        {
            var starUnitApi = this.Helper.ModRegistry.GetApi<IStarUnitApi>("Phrasefable.StarUnit");
            this._factory = starUnitApi.TestDefinitionFactory;

            starUnitApi.Register("aa", this.GetTestNodes().ToArray());
        }

        private IEnumerable<ITraversable> GetTestNodes()
        {
            yield return new HeldSeedsTests(this._factory).Build();
            yield return new SpreadingTests(this._factory).Build();
            yield return new GrowthTests(this._factory).Build();
            yield return new TreeUtilsTests(this._factory).Build();
        }
    }
}