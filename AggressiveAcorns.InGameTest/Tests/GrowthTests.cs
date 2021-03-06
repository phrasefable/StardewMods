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
    internal class GrowthTests
    {
        private readonly ITestDefinitionFactory _factory;

        private MutableConfigAdaptor _config;


        public GrowthTests(ITestDefinitionFactory factory)
        {
            this._factory = factory;
        }


        public ITraversable Build()
        {
            ITestFixtureBuilder fixtureBuilder = this._factory.CreateFixtureBuilder();

            fixtureBuilder.Key = "growth";

            fixtureBuilder.AddCondition(this._factory.Conditions.WorldReady);

            fixtureBuilder.BeforeAll = () =>
            {
                Game1.player.warpFarmer(LocationUtils.WarpFarm);
                Season.Spring.SetSeason();
            };

            fixtureBuilder.BeforeAllDelay = Delay.Second;
            // fixtureBuilder.AfterAll = () => Game1.player.warpFarmer(LocationUtils.WarpFarm);
            // fixtureBuilder.AfterAllDelay = Delay.Second;

            fixtureBuilder.BeforeEach = () =>
            {
                this._config = new MutableConfigAdaptor();
                AggressiveAcorns.Config = this._config;
            };

            fixtureBuilder.Delay = Delay.Tick;

            fixtureBuilder.AddChild(this.BuildTest_TreeGrows());
            fixtureBuilder.AddChild(this.BuildTest_StageSequence());
            fixtureBuilder.AddChild(this.BuildTest_GrowthChanceObeysConfig());
            fixtureBuilder.AddChild(this.BuildTest_ShadeObeysConfig());

            return fixtureBuilder.Build();
        }


        private ITestResult UpdateAndCheckHasGrown(Tree tree, bool expectGrowth)
        {
            int expectedStage = tree.growthStage.Value;
            if (expectGrowth) expectedStage += 1;
            return this.UpdateAndCheckHasGrown(tree, expectedStage);
        }


        private ITestResult UpdateAndCheckHasGrown(Tree tree, int expectedStage)
        {
            // Act
            tree.Update();

            // Assert
            return tree.growthStage.Value == expectedStage
                ? this._factory.BuildTestResult(Status.Pass)
                : this._factory.BuildTestResult(
                    Status.Fail,
                    $"Expected stage {expectedStage}, got {tree.growthStage.Value}."
                );
        }


        // ========== Grows, Overriding random =========================================================================

        private ITraversable BuildTest_TreeGrows()
        {
            ICasedTestBuilder<(double GrowthChance, bool ForcedValue)> testBuilder =
                this._factory.CreateCasedTestBuilder<(double, bool)>();

            testBuilder.Key = "tree_grows";
            testBuilder.TestMethod = this.Test_TreeGrows;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args =>
                $"chance_{(int) (args.GrowthChance * 100)}_force_{(args.ForcedValue ? "en" : "dis")}abled";
            testBuilder.AddCases(
                (GrowthChance: 0.00, ForcedValue: false),
                (GrowthChance: 0.01, ForcedValue: false),
                (GrowthChance: 0.99, ForcedValue: false),
                (GrowthChance: 1.00, ForcedValue: false),
                (GrowthChance: 0.00, ForcedValue: true),
                (GrowthChance: 0.01, ForcedValue: true),
                (GrowthChance: 0.99, ForcedValue: true),
                (GrowthChance: 1.00, ForcedValue: true)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_TreeGrows((double, bool) args)
        {
            (double growthChance, bool forcedValue) = args;

            // Arrange
            this._config.DailyGrowthChance = growthChance;
            this._config.GrowthRoller = () => forcedValue;
            Tree tree = Utilities.TreeUtils.GetFarmTreeLonely(Tree.saplingStage);

            // Act, Assert
            return this.UpdateAndCheckHasGrown(tree, forcedValue);
        }


        // ========== Stages go in correct order =======================================================================

        private ITraversable BuildTest_StageSequence()
        {
            ICasedTestBuilder<(int InitialStage, int NextStage)> testBuilder =
                this._factory.CreateCasedTestBuilder<(int, int)>();

            testBuilder.Key = "next_stage";
            testBuilder.TestMethod = this.Test_StageSequence;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args => $"{args.InitialStage}";
            testBuilder.AddCases(
                (InitialStage: Tree.seedStage, NextStage: Tree.sproutStage),
                (InitialStage: Tree.sproutStage, NextStage: Tree.saplingStage),
                (InitialStage: Tree.saplingStage, NextStage: Tree.bushStage),
                (InitialStage: Tree.bushStage, NextStage: Tree.bushStage + 1),
                (InitialStage: Tree.bushStage + 1, NextStage: Tree.treeStage),
                (InitialStage: Tree.treeStage, NextStage: Tree.treeStage)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_StageSequence((int, int) args)
        {
            (int initialStage, int nextStage) = args;

            // Arrange
            this._config.GrowthRoller = () => true;

            // Act, Assert
            return this.UpdateAndCheckHasGrown(Utilities.TreeUtils.GetFarmTreeLonely(initialStage), nextStage);
        }


        // ========== Grows, varying config chance =====================================================================

        private ITraversable BuildTest_GrowthChanceObeysConfig()
        {
            ICasedTestBuilder<(double GrowthChance, bool ExpectGrowth)> testBuilder =
                this._factory.CreateCasedTestBuilder<(double, bool)>();

            testBuilder.Key = "config_chance";
            testBuilder.TestMethod = this.Test_GrowthChanceObeysConfig;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args => $"{args.ExpectGrowth}";
            testBuilder.AddCases(
                (GrowthChance: 0.0, ExpectGrowth: false),
                (GrowthChance: 1.0, ExpectGrowth: true)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_GrowthChanceObeysConfig((double, bool) args)
        {
            (double growthChance, bool expectGrowth) = args;

            // Arrange
            this._config.DailyGrowthChance = growthChance;
            Tree tree = Utilities.TreeUtils.GetFarmTreeLonely(Tree.saplingStage);

            // Act, Assert
            return this.UpdateAndCheckHasGrown(tree, expectGrowth);
        }


        // ========== Growth respects max shaded stage config option ===================================================

        private ITraversable BuildTest_ShadeObeysConfig()
        {
            ICasedTestBuilder<(int MaxShadedGrowthStage, int InitStage, bool ExpectGrowth)> testBuilder =
                this._factory.CreateCasedTestBuilder<(int, int, bool)>();

            testBuilder.Key = "shade_config";
            testBuilder.TestMethod = this.Test_ShadeObeysConfig;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args => $"init_{args.InitStage}_max_{args.MaxShadedGrowthStage}";

            for (int maxShaded = Tree.seedStage; maxShaded <= Tree.treeStage; maxShaded++)
            {
                for (int initStage = Tree.seedStage; initStage <= Tree.treeStage; initStage++)
                {
                    testBuilder.AddCases(
                        (MaxShadedGrowthStage: maxShaded, InitStage: initStage, ExpectGrowth: initStage < maxShaded)
                    );
                }
            }

            return testBuilder.Build();
        }


        private ITestResult Test_ShadeObeysConfig((int, int, bool) args)
        {
            (int maxShadedGrowthStage, int initStage, bool expectGrowth) = args;

            // Arrange
            this._config.GrowthRoller = () => true;
            this._config.MaxShadedGrowthStage = maxShadedGrowthStage;
            Tree tree = Utilities.TreeUtils.GetFarmTreeLonely(initStage);
            Utilities.TreeUtils.PlantTree(
                tree.currentLocation,
                tree.currentTileLocation - new Vector2(-1, 0),
                tree.treeType.Value,
                Tree.treeStage
            );

            // Act, Assert
            return this.UpdateAndCheckHasGrown(tree, expectGrowth);
        }
    }
}
