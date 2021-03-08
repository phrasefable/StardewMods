using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Utilities;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using StardewValley;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Tests
{
    internal partial class SpreadingTests
    {
        private ITraversable BuildFixture_Seasonal()
        {
            string initialSeason = null;

            ITestFixtureBuilder builder = this._factory.CreateFixtureBuilder();

            builder.Key = "spreading_seasonal";

            // Not needed while sub-fixture of SpreadingTests:
            // builder.AddCondition(this._factory.Conditions.WorldReady);
            // fixtureBuilder.BeforeAll = () => Game1.player.warpFarmer(Utils.WarpFarm);

            builder.BeforeAll = () => initialSeason = Game1.currentSeason;
            builder.BeforeAllDelay = Delay.Second;
            builder.AfterAll = () => SeasonUtils.SetSeason(initialSeason);

            builder.BeforeEach = () =>
            {
                this._config = new MutableConfigAdaptor();
                AggressiveAcorns.Config = this._config;

                this._seedLocator = new SeedLocator();
                this._config.SpreadOffsetGenerator = () => this._seedLocator.GenerateOffsets();
            };

            builder.Delay = Delay.Tick;

            builder.AddChild(this.BuildTest_SeasonalSpreading());

            return builder.Build();
        }


        // ========== Seasonal Spread  ========================================================================

        private ITraversable BuildTest_SeasonalSpreading()
        {
            ICasedTestBuilder<(Season Season, bool AllowWinterSpread, bool ExpectSpread)> testBuilder =
                this._factory.CreateCasedTestBuilder<(Season, bool, bool)>();

            testBuilder.Key = "seasonal_spreading";
            testBuilder.TestMethod = this.Test_SeasonalSpreading;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args =>
                $"{args.Season.GetName()}_{(args.AllowWinterSpread ? "with" : "without")}_winter_spread";
            testBuilder.AddCases(
                (Season: Season.Spring, false, true),
                (Season: Season.Summer, false, true),
                (Season: Season.Fall, false, true),
                (Season: Season.Winter, false, false)
            );
            testBuilder.AddCases(
                (Season: Season.Spring, true, true),
                (Season: Season.Summer, true, true),
                (Season: Season.Fall, true, true),
                (Season: Season.Winter, true, true)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_SeasonalSpreading((Season, bool, bool) args)
        {
            (Season season, bool allowWinterSpread, bool expectSpread) = args;

            // Arrange
            season.SetSeason();
            this._config.ChanceSpread = 1.0;
            this._config.DoSpreadInWinter = allowWinterSpread;

            // Act, Assert
            ITestResult result = this.UpdateAndCheckTreeHasSpread(
                Utilities.TreeUtils.GetFarmTreeLonely(),
                expectSpread
            );
            return result;
        }
    }
}