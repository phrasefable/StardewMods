using Microsoft.Xna.Framework;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Tests
{
    public class SpreadingTests
    {
        private readonly ITestDefinitionFactory _factory;

        private MutableConfigAdaptor _config;


        public SpreadingTests(ITestDefinitionFactory factory)
        {
            this._factory = factory;
        }


        public ITraversable Build()
        {
            ITestFixtureBuilder fixtureBuilder = _factory.CreateFixtureBuilder();

            fixtureBuilder.Key = "spreading";

            fixtureBuilder.AddCondition(this._factory.Conditions.WorldReady);

            /* Note warps are not synchronous - printing the players position before and after the warp
             * statement does not show a difference. Still seems to work (the locations used in this test are
             * always loaded??), so have not bothered to make the test framework asynchronous.
             * */
            fixtureBuilder.BeforeAll = () => Game1.player.warpFarmer(Utils.WarpFarm);
            fixtureBuilder.BeforeAllDelay = Delay.Second;

            fixtureBuilder.BeforeEach = () =>
            {
                this._config = new MutableConfigAdaptor();
                AggressiveAcorns.Config = this._config;
            };

            fixtureBuilder.Delay = Delay.Tick;

            fixtureBuilder.AddChild(this.BuildTest_TreeSpreads());
            fixtureBuilder.AddChild(this.BuildTest_TreeSpreads_Override());
            fixtureBuilder.AddChild(this.BuildTest_StumpNeverSpreads());

            return fixtureBuilder.Build();
        }


        private ITestResult UpdateAndCheckTreeHasSpread(Tree tree, bool expectSpread)
        {
            // Act
            tree.Update();

            // Assert
            var foundSeeds = 0;
            foreach (Vector2 possiblePosition in Common.Utilities.GetTilesInRadius(tree.currentTileLocation, 3))
            {
                if (possiblePosition == tree.currentTileLocation) continue;

                if (tree.currentLocation.terrainFeatures.TryGetValue(possiblePosition, out TerrainFeature feature) &&
                    feature is Tree possibleSeed)
                {
                    if (SpreadingTests.CouldBeSeedOf(tree, possibleSeed)) foundSeeds++;
                }
            }

            int expectedSeeds = expectSpread ? 1 : 0;
            return foundSeeds == expectedSeeds
                ? this._factory.BuildTestResult(Status.Pass)
                : this._factory.BuildTestResult(Status.Fail, $"Found {foundSeeds} seeds, expected {expectedSeeds}");
        }


        private static bool CouldBeSeedOf(Tree tree, Tree possibleSeed)
        {
            return possibleSeed.stump.Value == false &&
                   possibleSeed.growthStage.Value == Tree.seedStage &&
                   possibleSeed.treeType == tree.treeType;
        }


        // ========== Spreads, varying config chance ==================================================================

        private ITraversable BuildTest_TreeSpreads()
        {
            ICasedTestBuilder<(double SpreadChance, bool ExpectSeed)> testBuilder =
                this._factory.CreateCasedTestBuilder<(double, bool)>();

            testBuilder.Key = "tree_spreads";
            testBuilder.TestMethod = this.Test_TreeSpreads;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args => $"chance_{args.SpreadChance}";
            testBuilder.AddCases(
                (SpreadChance: 0.0, ExpectSeed: false),
                (SpreadChance: 1.0, ExpectSeed: true)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_TreeSpreads((double, bool) args)
        {
            (double spreadChance, bool expectSpread) = args;

            // Arrange
            this._config.DailySpreadChance = spreadChance;

            // Act, Assert
            return this.UpdateAndCheckTreeHasSpread(Utils.GetFarmTreeLonely(), expectSpread);
        }


        // ========== Held seed, overriding random function ============================================================

        private ITraversable BuildTest_TreeSpreads_Override()
        {
            ICasedTestBuilder<(double Chance, bool ExpectSpread)> testBuilder =
                _factory.CreateCasedTestBuilder<(double, bool)>();

            testBuilder.Key = "tree_spreads_override_random";
            testBuilder.TestMethod = this.Test_TreeSpreads_Override;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = @case => $"chance_{@case.Chance * 100}_random_always_{@case.ExpectSpread}";
            testBuilder.AddCases(
                (Chance: 0.0, ExpectSpread: true),
                (Chance: 0.5, ExpectSpread: true),
                (Chance: 1.0, ExpectSpread: true),
                (Chance: 0.0, ExpectSpread: false),
                (Chance: 0.5, ExpectSpread: false),
                (Chance: 1.0, ExpectSpread: false)
            );
            return testBuilder.Build();
        }


        private ITestResult Test_TreeSpreads_Override((double Chance, bool ExpectSeed) @params)
        {
            (double configChance, bool expectSpread) = @params;

            // Arrange
            this._config.DailySpreadChance = configChance;
            this._config.SpreadRoller = () => expectSpread;

            // Act, assert
            return this.UpdateAndCheckTreeHasSpread(Utils.GetFarmTreeLonely(), expectSpread);
        }


        // ========== Stumps never spread ==============================================================================

        private ITraversable BuildTest_StumpNeverSpreads()
        {
            ITestBuilder testBuilder = this._factory.CreateTestBuilder();

            testBuilder.Key = "stump_doesnt_spread";
            testBuilder.Delay = Delay.Tick;
            testBuilder.TestMethod = this.Test_StumpNeverSpreads;

            return testBuilder.Build();
        }


        private ITestResult Test_StumpNeverSpreads()
        {
            // Arrange
            Tree tree = Utils.GetFarmTreeLonely();
            tree.stump.Value = true;

            this._config.DailySpreadChance = 1.0;
            this._config.SpreadRoller = () => true;

            // Act, assert
            return this.UpdateAndCheckTreeHasSpread(tree, false);
        }
    }
}
