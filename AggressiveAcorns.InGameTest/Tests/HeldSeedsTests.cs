using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Utilities;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Tests
{
    internal class HeldSeedsTests
    {
        private readonly ITestDefinitionFactory _factory;

        private MutableConfigAdaptor _config;


        public HeldSeedsTests(ITestDefinitionFactory factory)
        {
            this._factory = factory;
        }


        public ITraversable Build()
        {
            ITestFixtureBuilder fixtureBuilder = _factory.CreateFixtureBuilder();

            fixtureBuilder.Key = "held_seeds";

            fixtureBuilder.AddCondition(this._factory.Conditions.WorldReady);

            /* Note warps are not synchronous - printing the players position before and after the warp
             * statement does not show a difference. Still seems to work (the locations used in this test are
             * always loaded??), so have not bothered to make the test framework asynchronous.
             * */
            fixtureBuilder.BeforeAll = () => Game1.player.warpFarmer(LocationUtils.WarpFarm);
            fixtureBuilder.BeforeAllDelay = Delay.Second;

            fixtureBuilder.BeforeEach = () =>
            {
                this._config = new MutableConfigAdaptor {DailySpreadChance = 0.0};
                AggressiveAcorns.Config = this._config;
            };

            fixtureBuilder.AddChild(this.BuildTest_HeldSeed());
            fixtureBuilder.AddChild(this.BuildTest_HeldSeed_Override());
            fixtureBuilder.AddChild(this.BuildTest_SeedPersistence());
            fixtureBuilder.AddChild(this.BuildTest_HeldSeed_ByStage());
            fixtureBuilder.AddChild(this.BuildTest_HeldSeed_StumpByStage());
            fixtureBuilder.AddChild(this.BuildTest_HeldSeed_InAllLocations());

            return fixtureBuilder.Build();
        }


        private ITestResult CheckTreeHasSeedAfterUpdate(Tree tree, bool expectSeed)
        {
            // Act
            tree.Update();

            // Assert
            return tree.hasSeed.Value == expectSeed
                ? this._factory.BuildTestResult(Status.Pass)
                : this._factory.BuildTestResult(Status.Fail, expectSeed ? "Seed expected" : "Seed not expected");
        }


        // ========== Held seed, varying config chance ================================================================

        private ITraversable BuildTest_HeldSeed()
        {
            ICasedTestBuilder<(double SeedChance, bool ExpectSeed)> testBuilder =
                _factory.CreateCasedTestBuilder<(double, bool)>();

            testBuilder.Key = "tree_holds_seeds";
            testBuilder.TestMethod = this.Test_HeldSeed;
            testBuilder.KeyGenerator = @case => $"chance_{@case.SeedChance * 100}";
            testBuilder.AddCases(
                (SeedChance: 0.0, ExpectSeed: false),
                (SeedChance: 1.0, ExpectSeed: true)
            );
            return testBuilder.Build();
        }


        private ITestResult Test_HeldSeed((double, bool) @params)
        {
            (double seedChance, bool expectSeed) = @params;

            // Arrange
            this._config.DailySeedChance = seedChance;

            // Act, assert
            return CheckTreeHasSeedAfterUpdate(Utilities.TreeUtils.GetFarmTreeLonely(), expectSeed);
        }


        // ========== Held seed, overriding random function ============================================================

        private ITraversable BuildTest_HeldSeed_Override()
        {
            ICasedTestBuilder<(double Chance, bool ExpectSeed)> testBuilder =
                _factory.CreateCasedTestBuilder<(double, bool)>();

            testBuilder.Key = "tree_holds_seeds_override_random";
            testBuilder.TestMethod = this.Test_HeldSeed_Override;
            testBuilder.KeyGenerator = @case => $"chance_{@case.Chance * 100}_random_always_{@case.ExpectSeed}";
            testBuilder.AddCases(
                (Chance: 0.0, ExpectSeed: true),
                (Chance: 0.5, ExpectSeed: true),
                (Chance: 1.0, ExpectSeed: true),
                (Chance: 0.0, ExpectSeed: false),
                (Chance: 0.5, ExpectSeed: false),
                (Chance: 1.0, ExpectSeed: false)
            );
            return testBuilder.Build();
        }


        private ITestResult Test_HeldSeed_Override((double Chance, bool ExpectSeed) @params)
        {
            (double configChance, bool expectSeed) = @params;

            // Arrange
            this._config.DailySeedChance = configChance;
            this._config.SeedRoller = () => expectSeed;

            // Act, assert
            return this.CheckTreeHasSeedAfterUpdate(Utilities.TreeUtils.GetFarmTreeLonely(), expectSeed);
        }


        // ========== Seed persistence =================================================================================

        private ITraversable BuildTest_SeedPersistence()
        {
            ICasedTestBuilder<(bool DoPersist, bool InitialSeed)> testBuilder =
                _factory.CreateCasedTestBuilder<(bool, bool)>();

            testBuilder.Key = "seed_persistence";
            testBuilder.TestMethod = this.Test_SeedPersistence;
            testBuilder.KeyGenerator = @case =>
                $"{(@case.DoPersist ? "do" : "dont")}_persist_with{(@case.InitialSeed ? "" : "out")}_initial_seed";
            testBuilder.AddCases(
                (DoPersist: true, InitialSeed: true),
                (DoPersist: true, InitialSeed: false),
                (DoPersist: false, InitialSeed: true),
                (DoPersist: false, InitialSeed: false)
            );
            return testBuilder.Build();
        }


        private ITestResult Test_SeedPersistence((bool DoPersist, bool InitialSeed) @params)
        {
            (bool doPersist, bool initialSeed) = @params;
            bool expectSeed = doPersist && initialSeed;

            // Arrange
            this._config.DailySeedChance = 0;
            this._config.DoSeedsPersist = doPersist;

            Tree tree = Utilities.TreeUtils.GetFarmTreeLonely();
            tree.hasSeed.Value = initialSeed;

            // Act, assert
            return this.CheckTreeHasSeedAfterUpdate(tree, expectSeed);
        }


        // ========== No seeds for immature trees ======================================================================

        private ITraversable BuildTest_HeldSeed_ByStage()
        {
            ICasedTestBuilder<(int Stage, bool ExpectSeed)>
                testBuilder = _factory.CreateCasedTestBuilder<(int, bool)>();

            testBuilder.Key = "immature_trees_dont_hold_seeds";
            testBuilder.TestMethod = this.Test_HeldSeed_ByStage;
            testBuilder.KeyGenerator = @case => $"stage_{@case.Stage}";
            testBuilder.AddCases(
                (Stage: Tree.seedStage, ExpectSeed: false),
                (Stage: Tree.sproutStage, ExpectSeed: false),
                (Stage: Tree.saplingStage, ExpectSeed: false),
                (Stage: Tree.bushStage, ExpectSeed: false),
                (Stage: Tree.bushStage + 1, ExpectSeed: false),
                (Stage: Tree.treeStage, ExpectSeed: true)
            );
            return testBuilder.Build();
        }


        private ITestResult Test_HeldSeed_ByStage((int Stage, bool ExpectSeed) @params)
        {
            (int stage, bool expectSeed) = @params;

            // Arrange
            this._config.DailySeedChance = 1.0;

            Tree tree = Utilities.TreeUtils.GetFarmTreeLonely();
            tree.growthStage.Value = stage;

            // Act, assert
            return this.CheckTreeHasSeedAfterUpdate(tree, expectSeed);
        }


        // ========== No seeds for immature trees ======================================================================

        private ITraversable BuildTest_HeldSeed_StumpByStage()
        {
            ICasedTestBuilder<int> testBuilder = _factory.CreateCasedTestBuilder<int>();

            testBuilder.Key = "stumps_dont_hold_seeds";
            testBuilder.TestMethod = this.Test_HeldSeed_ByStage;
            testBuilder.KeyGenerator = @case => $"stump_stage_{@case}";
            testBuilder.AddCases(
                Tree.seedStage,
                Tree.sproutStage,
                Tree.saplingStage,
                Tree.bushStage,
                Tree.bushStage + 1,
                Tree.treeStage
            );
            return testBuilder.Build();
        }


        private ITestResult Test_HeldSeed_ByStage(int stage)
        {
            // Arrange
            this._config.DailySeedChance = 1.0;

            Tree tree = Utilities.TreeUtils.GetFarmTreeLonely();
            tree.growthStage.Value = stage;
            tree.stump.Value = true;

            // Act, assert
            return this.CheckTreeHasSeedAfterUpdate(tree, false);
        }


        // ========== Only trees on the farm spread ====================================================================

        private ITraversable BuildTest_HeldSeed_InAllLocations()
        {
            ICasedTestBuilder<Warp> testBuilder = this._factory.CreateCasedTestBuilder<Warp>();

            testBuilder.Key = "locations";
            testBuilder.TestMethod = this.Test_HeldSeedInAllLocations;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = warp => $"{warp.TargetName}";
            testBuilder.AddCases(
                LocationUtils.WarpFarm,
                LocationUtils.WarpDesert,
                LocationUtils.WarpGreenhouse,
                LocationUtils.WarpBackwoods,
                LocationUtils.WarpRailroad,
                LocationUtils.WarpMountain,
                LocationUtils.WarpFarmCave,
                LocationUtils.WarpCellar4,
                LocationUtils.WarpTown,
                LocationUtils.WarpBeach,
                LocationUtils.WarpBusStop,
                LocationUtils.WarpWoods,
                LocationUtils.WarpForest
            );

            return testBuilder.Build();
        }


        private ITestResult Test_HeldSeedInAllLocations(Warp warp)
        {
            // Arrange
            Game1.player.warpFarmer(warp);
            Tree tree = Utilities.TreeUtils.GetLonelyTree(warp);
            this._config.DailySeedChance = 1.0;

            // Act, Assert
            return this.CheckTreeHasSeedAfterUpdate(tree, true);
        }
    }
}
