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

            return fixtureBuilder.Build();
        }


        private ITestResult UpdateAndCheckTreeHasSpread(Tree tree, bool expectSpread)
        {
            // Act
            tree.Update();

            // Assert
            if (expectSpread)
            {
                if (this._seedLocator.GeneratedOffsets.Count == 1 &&
                    SpreadingTests.CouldBeSeedOf(tree, this._seedLocator.GeneratedOffsets.First()))
                {
                    return this._factory.BuildTestResult(Status.Pass);
                }
            }
            else
            {
                if (this._seedLocator.GeneratedOffsets.Count == 0) return this._factory.BuildTestResult(Status.Pass);
            }

            return this._factory.BuildTestResult(Status.Fail);
        }


        private static bool CouldBeSeedOf(Tree tree, Vector2 offset)
        {
            Vector2 position = tree.currentTileLocation + offset;
            if (!tree.currentLocation.terrainFeatures.TryGetValue(position, out TerrainFeature feature)) return false;
            if (!(feature is Tree possibleSeed)) return false;
            return possibleSeed.growthStage.Value == Tree.seedStage &&
                   possibleSeed.treeType == tree.treeType;
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
    }
}
