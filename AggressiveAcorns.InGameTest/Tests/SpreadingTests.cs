using System.Collections.Generic;
using System.Linq;
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
            fixtureBuilder.AddChild(this.BuildFixture_Seasonal());
            fixtureBuilder.AddChild(this.BuildTest_SpreadOverGrass());

            return fixtureBuilder.Build();
        }


        private ITestResult UpdateAndCheckTreeHasSpread(Tree tree, bool expectSpread)
        {
            // Act
            tree.Update();

            // Assert
            if (!expectSpread && this._seedLocator.GeneratedOffsets.Count == 0)
            {
                return this._factory.BuildTestResult(Status.Pass);
            }

            IList<(bool IsSeed, Vector2 Offset, string Message)> potentialSeeds = this._seedLocator.GeneratedOffsets
                .Select(
                    offset => (
                        IsSeed: SpreadingTests.CouldBeSeedOf(tree, offset, out string message),
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
            Vector2 position = tree.currentTileLocation + offset;
            if (!tree.currentLocation.terrainFeatures.TryGetValue(position, out TerrainFeature feature))
            {
                message = "No seed present at expected position.";
                return false;
            }

            if (!(feature is Tree possibleSeed))
            {
                message = "No seed present at expected position.";
                return false;
            }

            var result = true;
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
            this._config.DailySpreadChance = spreadChance;

            // Act, Assert
            return this.UpdateAndCheckTreeHasSpread(Utilities.TreeUtils.GetFarmTreeLonely(), expectSpread);
        }


        // ========== Held seed, overriding random function ============================================================

        private ITraversable BuildTest_TreeSpreads_Override()
        {
            ICasedTestBuilder<(double Chance, bool ExpectSpread)> testBuilder =
                _factory.CreateCasedTestBuilder<(double, bool)>();

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
            this._config.DailySpreadChance = configChance;
            this._config.SpreadRoller = () => expectSpread;

            // Act, assert
            return this.UpdateAndCheckTreeHasSpread(Utilities.TreeUtils.GetFarmTreeLonely(), expectSpread);
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
            Tree tree = Utilities.TreeUtils.GetFarmTreeLonely();
            tree.stump.Value = true;

            this._config.DailySpreadChance = 1.0;
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
            Tree tree = Utilities.TreeUtils.GetFarmTreeLonely(growthStage);
            this._config.DailySpreadChance = 1.0;

            // Act, Assert
            return this.UpdateAndCheckTreeHasSpread(tree, expectSpread);
        }


        // ========== Spread seeds do/don't replace long grass ===========================================================

        private ITraversable BuildTest_SpreadOverGrass()
        {
            ICasedTestBuilder<bool> testBuilder = this._factory.CreateCasedTestBuilder<bool>();

            testBuilder.Key = "tree_spreads_over_grass";
            testBuilder.TestMethod = this.Test_SpreadOverGrass;
            testBuilder.Delay = Delay.Second;
            testBuilder.KeyGenerator = spreadOverGrass => $"{(spreadOverGrass ? "do" : "dont")}_replace_grass";
            testBuilder.AddCases(true, false);

            return testBuilder.Build();
        }


        private ITestResult Test_SpreadOverGrass(bool spreadOverGrass)
        {
            // Arrange
            Tree tree = Utilities.TreeUtils.GetFarmTreeLonely();
            this._config.DailySpreadChance = 1.0;
            this._config.SeedsReplaceGrass = spreadOverGrass;

            var radius = 5;
            for (int dX = -radius; dX <= radius; dX++)
            {
                for (int dY = -radius; dY < radius; dY++)
                {
                    if (dX == 0 && dY == 0) continue;
                    var position = new Vector2(tree.currentTileLocation.X + dX, tree.currentTileLocation.Y + dY);
                    tree.currentLocation.terrainFeatures[position] = new Grass(Grass.springGrass, 4);
                }
            }

            // Act, Assert
            return this.UpdateAndCheckTreeHasSpread(tree, spreadOverGrass);
        }
    }
}
