using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Decorators;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Tests
{
    internal class Seed_Tests
    {
        private readonly ITestDefinitionFactory _factory;

        private MutableConfigAdaptor _config;


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
                this._config = new MutableConfigAdaptor {DailySpreadChance = 0.0};
                AggressiveAcorns.Config = this._config;
            };

            fixtureBuilder.AddChild(this.BuildTest_HeldSeed());
            fixtureBuilder.AddChild(this.BuildTest_HeldSeed_Override());

            return fixtureBuilder.Build();
        }


        private ITestResult CheckTreeHasSeedAfterUpdate(Tree tree, bool expectSeed)
        {
            // Act
            tree.Update();

            // Assert
            return tree.hasSeed.Value == expectSeed
                ? this._factory.BuildTestResult(Status.Pass, null)
                : this._factory.BuildTestResult(Status.Fail, expectSeed ? "Seed expected" : "Seed not expected");
        }


        // ========== Held seed, varying config chance ================================================================

        private ITraversable BuildTest_HeldSeed()
        {
            ICasedTestBuilder<DoubleToBool> testBuilder = _factory.CreateCasedTestBuilder<DoubleToBool>();

            testBuilder.Key = "tree_holds_seeds";
            testBuilder.TestMethod = this.Test_HeldSeed;
            testBuilder.KeyGenerator = @case => $"chance_{@case.Double * 100}";
            testBuilder.AddCases(
                new DoubleToBool(0.0, false),
                new DoubleToBool(1.0, true)
            );
            return testBuilder.Build();
        }


        private ITestResult Test_HeldSeed(DoubleToBool @params)
        {
            double seedChance = @params.Double;
            bool expectSeed = @params.Bool;

            // Arrange
            this._config.DailySeedChance = seedChance;

            // Arrange, act, assert
            return CheckTreeHasSeedAfterUpdate(Utils.GetFarmTreeLonely(), expectSeed);
        }


        // ========== Held seed, overriding random function ============================================================

        private ITraversable BuildTest_HeldSeed_Override()
        {
            ICasedTestBuilder<DoubleToBool> testBuilder = _factory.CreateCasedTestBuilder<DoubleToBool>();

            testBuilder.Key = "tree_holds_seeds_override_random";
            testBuilder.TestMethod = this.Test_HeldSeed_Override;
            testBuilder.KeyGenerator = @case => $"chance_{@case.Double * 100}_random_always_{@case.Bool}";
            testBuilder.AddCases(
                new DoubleToBool(0.0, true),
                new DoubleToBool(0.5, true),
                new DoubleToBool(1.0, true),
                new DoubleToBool(0.0, false),
                new DoubleToBool(0.5, false),
                new DoubleToBool(1.0, false)
            );
            return testBuilder.Build();
        }


        private ITestResult Test_HeldSeed_Override(DoubleToBool @params)
        {
            double configChance = @params.Double;
            bool expectSeed = @params.Bool;

            this._config.DailySeedChance = configChance;
            this._config.SeedRoller = () => expectSeed;

            return this.CheckTreeHasSeedAfterUpdate(Utils.GetFarmTreeLonely(), expectSeed);
        }
    }
}
