using Microsoft.Xna.Framework;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Model;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Tests
{
    internal class Seed_Tests
    {
        private readonly ITestDefinitionFactory _factory;

        private ModConfig _config;

        public Seed_Tests(ITestDefinitionFactory factory)
        {
            this._factory = factory;
        }

        public ITraversable Build()
        {
            ITestFixtureBuilder fixtureBuilder = _factory.CreateFixtureBuilder();

            fixtureBuilder.Key = "seed_tests";

            fixtureBuilder.AddCondition(this._factory.Conditions.WorldReady);

            /* Note warps are not synchronous - printing the players position before and after the warp
             * statement does not show a difference. Still seems to work (the locations used in this test are
             * always loaded??), so have not bothered to make the test framework asynchronous.
             * */
            fixtureBuilder.BeforeAll = () => Game1.player.warpFarmer(Utils.WarpFarm);
            fixtureBuilder.BeforeEach = () =>
            {
                this._config = new ModConfig();
                AggressiveAcorns.Config = this._config;
            };

            fixtureBuilder.AddChild(this.BuildTest_HeldSeed());

            return fixtureBuilder.Build();
        }

        private ITraversable BuildTest_HeldSeed()
        {
            ICasedTestBuilder<DoubleToBool> testBuilder = _factory.CreateCasedTestBuilder<DoubleToBool>();

            testBuilder.Key = "tree_holds_seeds";
            testBuilder.TestMethod = this.TestHeldSeed;
            testBuilder.KeyGenerator = @case => $"chance_{@case.Double * 100}";
            testBuilder.AddCases(
                new DoubleToBool(0.0, false),
                new DoubleToBool(1.0, true)
            );
            return testBuilder.Build();
        }


        private ITestResult TestHeldSeed(DoubleToBool @params)
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
                ? this._factory.BuildTestResult(Status.Pass, null)
                : this._factory.BuildTestResult(Status.Fail, expectSeed ? "Seed expected" : "Seed not expected");
        }
    }
}
