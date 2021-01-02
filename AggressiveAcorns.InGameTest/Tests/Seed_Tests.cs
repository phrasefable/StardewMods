using Microsoft.Xna.Framework;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Builders;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Tests
{
    internal class Seed_Tests
    {
        private ModConfig _config;


        public ITestSuite GetFixture(IBuilderFactory factory)
        {
            ITestFixtureBuilder fixtureBuilder = factory.CreateFixtureBuilder();
            fixtureBuilder.SetKey("seed_tests");
            fixtureBuilder.AddCondition(Utils.Condition_WorldReady);
            fixtureBuilder.SetBeforeAllAction(
                /* Note warps are not synchronous - printing the players position before and after the warp
                 * statement does not show a difference. Still seems to work (the locations used in this test are
                 * always loaded??), so have not bothered to make the test framework asynchronous.
                 * */
                () => Game1.player.warpFarmer(Utils.WarpFarm)
            );
            fixtureBuilder.SetBeforeEachAction(
                () =>
                {
                    this._config = new ModConfig();
                    AggressiveAcorns.Config = this._config;
                }
            );

            fixtureBuilder.AddTest(this.GetTestBuilder_HeldSeed(factory));

            return fixtureBuilder.Build();
        }

        private Framework.Builders.ICasedTestBuilder<DoubleToBool> GetTestBuilder_HeldSeed(IBuilderFactory factory)
        {
            ICasedTestBuilder<DoubleToBool> testBuilder = factory.CreateCasedTestBuilder<DoubleToBool>();
            testBuilder.SetKey("tree_holds_seeds");
            testBuilder.SetTestMethod(this.TestHeldSeed);
            testBuilder.AddCases(
                new DoubleToBool(0.0, false),
                new DoubleToBool(1.0, true)
            );
            return testBuilder;
        }


        private IResult TestHeldSeed(DoubleToBool @params)
        {
            double seedChance = @params.Double;
            bool expectSeed = @params.Bool;

            // Arrange
            this._config.DailySeedChance = seedChance;
            Tree tree = Utils.PlantTree(
                Utils.WarpFarm.GetLocation(),
                Utils.WarpFarm.GetTargetTile() + new Vector2(0, -2),
                Tree.pineTree,
                Tree.treeStage
            );

            // Act
            tree.Update();

            // Assert
            return tree.hasSeed.Value == expectSeed
                ? new Result(Status.Pass)
                : new Result(Status.Fail, expectSeed ? "Seed expected" : "Seed not expected");
        }
    }
}
