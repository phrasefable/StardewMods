using System;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Utilities;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Tests
{
    internal partial class GrowthTests
    {
        private ITraversable BuildFixture_WinterGrowth()
        {
            ITestFixtureBuilder builder = this._factory.CreateFixtureBuilder();

            builder.Key = "winter_growth";

            // Not needed while sub-fixture of GrowthTests:
            // builder.AddCondition(this._factory.Conditions.WorldReady);
            // fixtureBuilder.BeforeAll = () => Game1.player.warpFarmer(Utils.WarpFarm);

            string initialSeason = null;
            builder.BeforeAll = () => initialSeason = Game1.currentSeason;
            builder.BeforeAllDelay = Delay.Second;
            builder.AfterAll = () =>
            {
                Game1.player.warpFarmer(LocationUtils.WarpFarm);
                SeasonUtils.SetSeason(initialSeason);
            };
            builder.AfterAllDelay = Delay.Second;

            builder.BeforeEach = () =>
            {
                this._config = new MutableConfigAdaptor();
                AggressiveAcorns.Config = this._config;
            };

            builder.Delay = Delay.Tick;

            builder.AddChild(this.BuildTest_WinterGrowthByLocation());
            builder.AddChild(this.BuildTest_WinterGrowthObeysConfig());

            return builder.Build();
        }


        // ========== Seasonal Growth - Farm == ========================================================================

        private ITraversable BuildTest_WinterGrowthObeysConfig()
        {
            ICasedTestBuilder<(Season Season, bool AllowWinterGrowth, int TreeType, bool ExpectGrowth)> testBuilder =
                this._factory.CreateCasedTestBuilder<(Season, bool, int, bool)>();

            testBuilder.Key = "winter_growth_config";
            testBuilder.TestMethod = this.Test_WinterGrowthObeysConfig;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args =>
                $"{args.Season.GetName()}_{(args.AllowWinterGrowth ? "with" : "without")}_winter_growth_" +
                args.TreeType switch
                {
                    Tree.bushyTree => "bushy",
                    Tree.leafyTree => "leafy",
                    Tree.pineTree => "pine",
                    Tree.palmTree => "palm",
                    _ => throw new ArgumentOutOfRangeException()
                };

            foreach (int treeType in new[] {Tree.bushyTree, Tree.leafyTree, Tree.pineTree, Tree.palmTree})
            {
                testBuilder.AddCases(
                    (Season: Season.Spring, AllowWinterGrowth: false, TreeType: treeType, ExpectGrowth: true),
                    (Season: Season.Summer, AllowWinterGrowth: false, TreeType: treeType, ExpectGrowth: true),
                    (Season: Season.Fall, AllowWinterGrowth: false, TreeType: treeType, ExpectGrowth: true),
                    (Season: Season.Winter, AllowWinterGrowth: false, TreeType: treeType, ExpectGrowth: false)
                );
                testBuilder.AddCases(
                    (Season: Season.Spring, AllowWinterGrowth: true, TreeType: treeType, ExpectGrowth: true),
                    (Season: Season.Summer, AllowWinterGrowth: true, TreeType: treeType, ExpectGrowth: true),
                    (Season: Season.Fall, AllowWinterGrowth: true, TreeType: treeType, ExpectGrowth: true),
                    (Season: Season.Winter, AllowWinterGrowth: true, TreeType: treeType, ExpectGrowth: true)
                );
            }

            return testBuilder.Build();
        }


        private ITestResult Test_WinterGrowthObeysConfig((Season, bool, int, bool) args)
        {
            (Season season, bool allowWinterGrowth, int treeType, bool expectGrowth) = args;

            // Arrange
            Game1.player.warpFarmer(LocationUtils.WarpFarm);
            season.SetSeason();
            this._config.GrowthRoller = () => true;
            this._config.DoGrowInWinter = allowWinterGrowth;

            Tree tree = Utilities.TreeUtils.GetFarmTreeLonely(Tree.seedStage, treeType);
            return this.UpdateAndCheckHasGrown(tree, expectGrowth);
        }


        // ========== Winter Growth ====================================================================================

        private ITraversable BuildTest_WinterGrowthByLocation()
        {
            ICasedTestBuilder<(Warp Location, bool ExpectGrowth)> testBuilder =
                this._factory.CreateCasedTestBuilder<(Warp, bool)>();

            testBuilder.Key = "no_winter";
            testBuilder.TestMethod = this.Test_WinterGrowthByLocation;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args => $"{args.Location.TargetName}";
            testBuilder.AddCases(
                (Location: LocationUtils.WarpFarm, ExpectGrowth: false),
                (Location: LocationUtils.WarpBackwoods, ExpectGrowth: false),
                (Location: LocationUtils.WarpRailroad, ExpectGrowth: false),
                (Location: LocationUtils.WarpMountain, ExpectGrowth: false),
                (Location: LocationUtils.WarpTown, ExpectGrowth: false),
                (Location: LocationUtils.WarpBeach, ExpectGrowth: false),
                (Location: LocationUtils.WarpBusStop, ExpectGrowth: false),
                (Location: LocationUtils.WarpWoods, ExpectGrowth: false),
                (Location: LocationUtils.WarpForest, ExpectGrowth: false)
            );
            testBuilder.AddCases(
                (Location: LocationUtils.WarpDesert, ExpectGrowth: true),
                (Location: LocationUtils.WarpGreenhouse, ExpectGrowth: true),
                (Location: LocationUtils.WarpFarmCave, ExpectGrowth: true),
                (Location: LocationUtils.WarpCellar4, ExpectGrowth: true)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_WinterGrowthByLocation((Warp Location, bool ExpectGrowth) args)
        {
            (Warp location, bool expectGrowth) = args;

            // Arrange
            Season.Winter.SetSeason();
            Game1.player.warpFarmer(location);
            this._config.GrowthRoller = () => true;
            this._config.DoGrowInWinter = false;

            Tree tree = Utilities.TreeUtils.GetLonelyTree(location, Tree.seedStage);
            return this.UpdateAndCheckHasGrown(tree, expectGrowth);
        }
    }
}
