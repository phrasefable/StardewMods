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
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += OnGameLoopOnGameLaunched;
        }

        private void OnGameLoopOnGameLaunched(object sender, GameLaunchedEventArgs args)
        {
            var starUnitApi = this.Helper.ModRegistry.GetApi<IStarUnitApi>("Phrasefable.StarUnit");
            starUnitApi.Register("aa", this.GetTestNodes(starUnitApi.TestDefinitionFactory));
        }

        private ITraversable[] GetTestNodes(ITestDefinitionFactory testDefinitionFactory)
        {
            return new ITraversable[]
            {
                new Seed_Tests(testDefinitionFactory).Build(),
                new TreeUtils_ExperiencesWinter_Test(testDefinitionFactory).Build()
            };
        }
    }
}