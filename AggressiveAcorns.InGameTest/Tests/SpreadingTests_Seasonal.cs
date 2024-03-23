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
            ICasedTestBuilder<(Utilities.Season Season, bool AllowWinterSpread, bool ExpectSpread)> testBuilder =
                this._factory.CreateCasedTestBuilder<(Utilities.Season, bool, bool)>();

            testBuilder.Key = "seasonal_spreading";
            testBuilder.TestMethod = this.Test_SeasonalSpreading;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args =>
                $"{args.Season.GetName()}_{(args.AllowWinterSpread ? "with" : "without")}_winter_spread";
            testBuilder.AddCases(
                (Season: Utilities.Season.Spring, false, true),
                (Season: Utilities.Season.Summer, false, true),
                (Season: Utilities.Season.Fall, false, true),
                (Season: Utilities.Season.Winter, false, false)
            );
            testBuilder.AddCases(
                (Season: Utilities.Season.Spring, true, true),
                (Season: Utilities.Season.Summer, true, true),
                (Season: Utilities.Season.Fall, true, true),
                (Season: Utilities.Season.Winter, true, true)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_SeasonalSpreading((Utilities.Season, bool, bool) args)
        {
            (Utilities.Season season, bool allowWinterSpread, bool expectSpread) = args;

            // Arrange
            season.SetSeason();
            this._config.ChanceSpread = 1.0;
            this._config.DoSpreadInWinter = allowWinterSpread;

            // Act, Assert
            ITestResult result = this.UpdateAndCheckTreeHasSpread(
                TreeUtils.GetFarmTreeLonely(),
                expectSpread
            );
            return result;
        }
    }
}