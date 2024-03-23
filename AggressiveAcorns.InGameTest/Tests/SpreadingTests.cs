using Microsoft.Xna.Framework;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Utilities;
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
        private readonly ITestDefinitionFactory _factory;

        private MutableConfigAdaptor _config;
        private SeedLocator _seedLocator;


        public SpreadingTests(ITestDefinitionFactory factory)
        {
            this._factory = factory;
        }


        public ITraversable Build()
        {
            ITestFixtureBuilder fixtureBuilder = this._factory.CreateFixtureBuilder();

            fixtureBuilder.Key = "spreading";

            fixtureBuilder.AddCondition(this._factory.Conditions.WorldReady);

            fixtureBuilder.BeforeAll = () => Game1.player.warpFarmer(LocationUtils.WarpFarm);
            fixtureBuilder.BeforeAllDelay = Delay.Second;
            fixtureBuilder.AfterAll = () => Game1.player.warpFarmer(LocationUtils.WarpFarm);
            fixtureBuilder.AfterAllDelay = Delay.Second;

            fixtureBuilder.BeforeEach = () =>
            {
                this._config = new MutableConfigAdaptor();
                AggressiveAcorns.Config = this._config;

                this._seedLocator = new SeedLocator();
                this._config.SpreadOffsetGenerator = () => this._seedLocator.GenerateOffsets();
            };

            fixtureBuilder.Delay = Delay.Tick;

            fixtureBuilder.AddChild(this.BuildTest_TreeSpreads());
            fixtureBuilder.AddChild(this.BuildTest_TreeSpreads_Override());
            fixtureBuilder.AddChild(this.BuildTest_StumpNeverSpreads());
            fixtureBuilder.AddChild(this.BuildTest_OnlyMatureTreesSpread());
            fixtureBuilder.AddChild(this.BuildTest_SpreadOverGrass());
            fixtureBuilder.AddChild(this.BuildTest_TappedSpread());
            fixtureBuilder.AddChild(this.BuildTest_TappedSpreadHeavy());
            fixtureBuilder.AddChild(this.BuildFixture_Seasonal());
            fixtureBuilder.AddChild(this.BuildTest_OnlyFarmTreesSpread());

            return fixtureBuilder.Build();
        }


        private ITestResult UpdateAndCheckTreeHasSpread(Tree tree, bool expectSpread)
        {
            // Act
            tree.dayUpdate();

            // Assert
            if (!expectSpread && this._seedLocator.GeneratedOffsets.Count == 0)
            {
                return this._factory.BuildTestResult(Status.Pass);
            }

            IList<(bool IsSeed, Vector2 Offset, string Message)> potentialSeeds = this._seedLocator.GeneratedOffsets
                .Select(
                    offset => (
                        IsSeed: CouldBeSeedOf(tree, offset, out string message),
                        Offset: offset,
                        Message: message
                    )
                )
                .ToList();

            IList<(Vector2 Offset, string Message)> validSeeds = potentialSeeds
                .Where(tuple => tuple.IsSeed)
                .Select(tuple => (tuple.Offset, tuple.Message))
                .ToList();

            string MakeMessage(string expected) => $"Expected {expected} but found {validSeeds.Count} seed(s).";

            if (!expectSpread)
            {
                return validSeeds.Count == 0
                    ? this._factory.BuildTestResult(Status.Pass)
                    : this._factory.BuildTestResult(Status.Fail, MakeMessage("0"));
            }

            // else expected spread
            if (validSeeds.Count == 1) return this._factory.BuildTestResult(Status.Pass);

            string failureReason = MakeMessage("exactly 1");
            if (validSeeds.Count == 0 && potentialSeeds.Count > 0)
            {
                foreach ((_, Vector2 offset, string message) in potentialSeeds)
                {
                    failureReason += $"\n\tOffset={offset}: {message}";
                }
            }

            return this._factory.BuildTestResult(Status.Fail, failureReason);
        }


        private static bool CouldBeSeedOf(Tree tree, Vector2 offset, out string message)
        {
            Vector2 position = tree.Tile + offset;
            if (!tree.Location.terrainFeatures.TryGetValue(position, out TerrainFeature feature))
            {
                message = "No seed present at expected position.";
                return false;
            }

            if (feature is not Tree possibleSeed)
            {
                message = "No seed present at expected position.";
                return false;
            }

            bool result = true;
            var messages = new List<string>(2);
            if (possibleSeed.growthStage.Value != Tree.seedStage)
            {
                messages.Add($"Not a seed; growthStage={possibleSeed.growthStage.Value}, expected={Tree.seedStage}.");
                result = false;
            }

            if (possibleSeed.treeType != tree.treeType)
            {
                messages.Add($"Incorrect type; treeType={possibleSeed.treeType.Value}, expected={tree.treeType}.");
                result = false;
            }

            message = string.Join(" ", messages);
            return result;
        }


        // ========== Spreads, varying config chance ==================================================================

        private ITraversable BuildTest_TreeSpreads()
        {
            ICasedTestBuilder<(double SpreadChance, bool ExpectSpread)> testBuilder =
                this._factory.CreateCasedTestBuilder<(double, bool)>();

            testBuilder.Key = "tree_spreads";
            testBuilder.TestMethod = this.Test_TreeSpreads;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args => $"chance_{args.SpreadChance}";
            testBuilder.AddCases(
                (SpreadChance: 0.0, ExpectSpread: false),
                (SpreadChance: 1.0, ExpectSpread: true)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_TreeSpreads((double, bool) args)
        {
            (double spreadChance, bool expectSpread) = args;

            // Arrange
            this._config.ChanceSpread = spreadChance;

            // Act, Assert
            return this.UpdateAndCheckTreeHasSpread(TreeUtils.GetFarmTreeLonely(), expectSpread);
        }


        // ========== Held seed, overriding random function ============================================================

        private ITraversable BuildTest_TreeSpreads_Override()
        {
            ICasedTestBuilder<(double Chance, bool ExpectSpread)> testBuilder =
                this._factory.CreateCasedTestBuilder<(double, bool)>();

            testBuilder.Key = "tree_spreads_override_random";
            testBuilder.TestMethod = this.Test_TreeSpreads_Override;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = @case => $"chance_{@case.Chance * 100}_random_always_{@case.ExpectSpread}";
            testBuilder.AddCases(
                (Chance: 0.0, ExpectSpread: true),
                (Chance: 0.5, ExpectSpread: true),
                (Chance: 1.0, ExpectSpread: true),
                (Chance: 0.0, ExpectSpread: false),
                (Chance: 0.5, ExpectSpread: false),
                (Chance: 1.0, ExpectSpread: false)
            );
            return testBuilder.Build();
        }


        private ITestResult Test_TreeSpreads_Override((double Chance, bool ExpectSpread) @params)
        {
            (double configChance, bool expectSpread) = @params;

            // Arrange
            this._config.ChanceSpread = configChance;
            this._config.SpreadRoller = () => expectSpread;

            // Act, assert
            return this.UpdateAndCheckTreeHasSpread(TreeUtils.GetFarmTreeLonely(), expectSpread);
        }


        // ========== Stumps never spread ==============================================================================

        private ITraversable BuildTest_StumpNeverSpreads()
        {
            ITestBuilder testBuilder = this._factory.CreateTestBuilder();

            testBuilder.Key = "stump_doesnt_spread";
            testBuilder.Delay = Delay.Tick;
            testBuilder.TestMethod = this.Test_StumpNeverSpreads;

            return testBuilder.Build();
        }


        private ITestResult Test_StumpNeverSpreads()
        {
            // Arrange
            Tree tree = TreeUtils.GetFarmTreeLonely();
            tree.stump.Value = true;

            this._config.ChanceSpread = 1.0;
            this._config.SpreadRoller = () => true;

            // Act, assert
            return this.UpdateAndCheckTreeHasSpread(tree, false);
        }


        // ========== Only mature trees spread  ========================================================================

        private ITraversable BuildTest_OnlyMatureTreesSpread()
        {
            ICasedTestBuilder<(int GrowthStage, bool ExpectSpread)> testBuilder =
                this._factory.CreateCasedTestBuilder<(int, bool)>();

            testBuilder.Key = "tree_spreads_per_stage";
            testBuilder.TestMethod = this.Test_OnlyMatureTreesSpread;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args => $"stage_{args.GrowthStage}";
            testBuilder.AddCases(
                (GrowthStage: Tree.seedStage, ExpectSpread: false),
                (GrowthStage: Tree.sproutStage, ExpectSpread: false),
                (GrowthStage: Tree.saplingStage, ExpectSpread: false),
                (GrowthStage: Tree.bushStage, ExpectSpread: false),
                (GrowthStage: Tree.treeStage, ExpectSpread: true)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_OnlyMatureTreesSpread((int, bool) args)
        {
            (int growthStage, bool expectSpread) = args;

            // Arrange
            Tree tree = TreeUtils.GetFarmTreeLonely(growthStage);
            this._config.ChanceSpread = 1.0;

            // Act, Assert
            return this.UpdateAndCheckTreeHasSpread(tree, expectSpread);
        }


        // ========== Spread seeds do/don't replace long grass ===========================================================

        private ITraversable BuildTest_SpreadOverGrass()
        {
            ICasedTestBuilder<bool> testBuilder = this._factory.CreateCasedTestBuilder<bool>();

            testBuilder.Key = "tree_spreads_over_grass";
            testBuilder.TestMethod = this.Test_SpreadOverGrass;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = spreadOverGrass => $"{(spreadOverGrass ? "do" : "dont")}_replace_grass";
            testBuilder.AddCases(true, false);

            return testBuilder.Build();
        }


        private ITestResult Test_SpreadOverGrass(bool spreadOverGrass)
        {
            // Arrange
            Tree tree = TreeUtils.GetFarmTreeLonely();
            this._config.ChanceSpread = 1.0;
            this._config.DoSeedsReplaceGrass = spreadOverGrass;

            int radius = 5;
            for (int dX = -radius; dX <= radius; dX++)
            {
                for (int dY = -radius; dY < radius; dY++)
                {
                    if (dX == 0 && dY == 0) continue;
                    var position = new Vector2(tree.Tile.X + dX, tree.Tile.Y + dY);
                    tree.Location.terrainFeatures[position] = new Grass(Grass.springGrass, 4);
                }
            }

            // Act, Assert
            return this.UpdateAndCheckTreeHasSpread(tree, spreadOverGrass);
        }


        // ========== Tapped Spreading =================================================================================

        private ITraversable BuildTest_TappedSpread()
        {
            ICasedTestBuilder<(bool TappedMaySpread, bool IsTapped, bool ExpectSpread)> testBuilder =
                this._factory.CreateCasedTestBuilder<(bool, bool, bool)>();

            testBuilder.Key = "tapped";
            testBuilder.TestMethod = this.Test_TappedSpread;
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


        private ITestResult Test_TappedSpread((bool TappedMaySpread, bool IsTapped, bool ExpectSpread) args)
        {
            (bool tappedMaySpread, bool isTapped, bool expectSpread) = args;

            // Arrange
            this._config.ChanceSpread = 1.0;
            this._config.DoTappedSpread = tappedMaySpread;

            Tree tree = TreeUtils.GetFarmTreeLonely();
            if (isTapped)
            {
                new StardewValley.Object(Vector2.Zero, "105").placementAction(
                    tree.Location,
                    (int) tree.Tile.X * 64,
                    (int) tree.Tile.Y * 64,
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


        // ========== Only trees on the farm spread ====================================================================

        private ITraversable BuildTest_OnlyFarmTreesSpread()
        {
            ICasedTestBuilder<(Warp Warp, bool ExpectSpread)> testBuilder =
                this._factory.CreateCasedTestBuilder<(Warp, bool)>();

            testBuilder.Key = "spreading_by_game_location";
            testBuilder.TestMethod = this.Test_OnlyFarmTreesSpread;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args => $"{args.Warp.TargetName}";
            testBuilder.AddCases(
                (Warp: LocationUtils.WarpFarm, ExpectSpread: true),
                (Warp: LocationUtils.WarpDesert, ExpectSpread: false),
                (Warp: LocationUtils.WarpGreenhouse, ExpectSpread: false),
                (Warp: LocationUtils.WarpBackwoods, ExpectSpread: false),
                (Warp: LocationUtils.WarpRailroad, ExpectSpread: false),
                (Warp: LocationUtils.WarpMountain, ExpectSpread: false),
                (Warp: LocationUtils.WarpFarmCave, ExpectSpread: false),
                (Warp: LocationUtils.WarpCellar, ExpectSpread: false),
                (Warp: LocationUtils.WarpTown, ExpectSpread: false),
                (Warp: LocationUtils.WarpBeach, ExpectSpread: false),
                (Warp: LocationUtils.WarpBusStop, ExpectSpread: false),
                (Warp: LocationUtils.WarpWoods, ExpectSpread: false),
                (Warp: LocationUtils.WarpForest, ExpectSpread: false)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_OnlyFarmTreesSpread((Warp Warp, bool ExpectSpread) args)
        {
            (Warp warp, bool expectSpread) = args;

            // Arrange
            Game1.player.warpFarmer(warp);
            Tree tree = TreeUtils.GetLonelyTree(warp);
            this._config.ChanceSpread = 1.0;

            // Act, Assert
            return this.UpdateAndCheckTreeHasSpread(tree, expectSpread);
        }
    }
}