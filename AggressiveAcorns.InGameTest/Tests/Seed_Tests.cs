using Microsoft.Xna.Framework;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework;
using Phrasefable.StardewMods.Common;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Tests
{
    internal class Seed_Tests
    {
        private ModConfig _config;
        private readonly TestSuite _suite;
        private Tree _tree;


        public Seed_Tests()
        {
            this._suite = new TestSuite {BeforeAll = BeforeAll, BeforeEach = BeforeEach};
            this._suite.Add(this.BuildBasicTest());
        }

        public ITest GetTest()
        {
            return this._suite.Guard_WorldReady();
        }


        private void BeforeEach()
        {
            this._config = new ModConfig();
            AggressiveAcorns.Config = this._config;
        }


        private void BeforeAll()
        {
            /* Note warps are not synchronous - printing the players position before and after the warp statement does
             * not show a difference. Still seems to work (the locations used in this test are always loaded??), so have
             * not bothered to make the test framework asynchronous.
             * */
            Game1.player.warpFarmer(Utils.WarpFarm);
        }


        private void PlantTestTree(
            GameLocation location,
            Vector2 position,
            int stage = Tree.treeStage,
            int type = Tree.pineTree)
        {
            this._tree = new AggressiveTree(new Tree(type, stage));

            foreach (Vector2 tile in Utilities.GetTilesInRadius(position, 3))
            {
                location.removeEverythingExceptCharactersFromThisTile((int) tile.X, (int) tile.Y);
            }

            location.terrainFeatures.Add(position, this._tree);
        }


        private ITest BuildBasicTest()
        {
            var cases = new CasedTest<double, bool>(
                "seeds_config",
                (seedChance, expectSeed) =>
                {
                    // Arrange
                    this._config.DailySeedChance = seedChance;
                    this.PlantTestTree(
                        Utils.WarpFarm.GetLocation(),
                        Utils.WarpFarm.GetTargetTile() + new Vector2(0, -2)
                    );

                    // Act
                    this._tree.Update();

                    // Assert
                    return this._tree.hasSeed.Value == expectSeed
                        ? new TestResult(TestOutcome.Pass)
                        : new TestResult(TestOutcome.Fail, expectSeed ? "Seed expected" : "Seed not expected");
                }
            );

            cases.AddCase(0.0, false);
            cases.AddCase(1.0, true);

            return cases;
        }
    }
}