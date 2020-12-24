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

        public Seed_Tests(ITestSuiteBuilder parentNode)
        {
            ICasedTestBuilder<DoubleToBool> testBuilder = parentNode.AddCasedTest<DoubleToBool>("tree_holds_seeds");

            testBuilder.AddCondition(Utils.Condition_WorldReady);

            testBuilder.SetBeforeAllAction(
                /* Note warps are not synchronous - printing the players position before and after the warp
                 * statement does not show a difference. Still seems to work (the locations used in this test are
                 * always loaded??), so have not bothered to make the test framework asynchronous.
                 * */
                () => Game1.player.warpFarmer(Utils.WarpFarm)
            );

            testBuilder.SetBeforeEachAction(
                () =>
                {
                    this._config = new ModConfig();
                    AggressiveAcorns.Config = this._config;
                }
            );

            testBuilder.SetTestMethod(this.TestHeldSeed);
            testBuilder.AddCases(
                new DoubleToBool(0.0, false),
                new DoubleToBool(1.0, true)
            );
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
