using Microsoft.Xna.Framework;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Tests
{
    internal partial class SpreadingTests
    {
        // ========== Tapped Spreading =================================================================================

        private ITraversable BuildTest_TappedSpreadHeavy()
        {
            ICasedTestBuilder<(bool TappedMaySpread, bool IsTapped, bool ExpectSpread)> testBuilder =
                this._factory.CreateCasedTestBuilder<(bool, bool, bool)>();

            testBuilder.Key = "tapped_heavy";
            testBuilder.TestMethod = this.Test_TappedSpreadHeavy;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args =>
                $"Is{(args.IsTapped ? "" : "_not")}_tapped_and_may{(args.TappedMaySpread ? "" : "_not")}_spread";
            testBuilder.AddCases(
                (TappedMaySpread: false, IsTapped: false, ExpectSpread: true),
                (TappedMaySpread: false, IsTapped: true, ExpectSpread: false),
                (TappedMaySpread: true, IsTapped: false, ExpectSpread: true),
                (TappedMaySpread: true, IsTapped: true, ExpectSpread: true)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_TappedSpreadHeavy((bool TappedMaySpread, bool IsTapped, bool ExpectSpread) args)
        {
            (bool tappedMaySpread, bool isTapped, bool expectSpread) = args;

            // Arrange
            this._config.ChanceSpread = 1.0;
            this._config.DoTappedSpread = tappedMaySpread;

            Tree tree = Utilities.TreeUtils.GetFarmTreeLonely();
            if (isTapped)
            {
                new Object(Vector2.Zero, 264).placementAction(
                    tree.currentLocation,
                    (int) tree.currentTileLocation.X * 64,
                    (int) tree.currentTileLocation.Y * 64,
                    Game1.player
                );
            }

            // Act, Assert
            if (isTapped != tree.tapped.Value)
            {
                return this._factory.BuildTestResult(
                    Status.Error,
                    tree.tapped.Value ? "Is tapped unexpectedly." : "Is not tapped when expected."
                );
            }

            return this.UpdateAndCheckTreeHasSpread(tree, expectSpread);
        }
    }
}
