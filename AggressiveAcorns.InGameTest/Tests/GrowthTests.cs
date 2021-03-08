using System;
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
    internal partial class GrowthTests
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

            fixtureBuilder.AfterAll = () =>
            {
                Game1.player.warpFarmer(LocationUtils.WarpFarm);
                Season.Spring.SetSeason();
            };
            fixtureBuilder.AfterAllDelay = Delay.Second;

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
            fixtureBuilder.AddChild(this.BuildTest_ShadePositions());
            fixtureBuilder.AddChild(this.BuildTest_ShadeSources());
            fixtureBuilder.AddChild(this.BuildTest_InstantGrowthObeysConfig());
            fixtureBuilder.AddChild(this.BuildTest_InstantGrowthShaded());
            fixtureBuilder.AddChild(this.BuildFixture_WinterGrowth());

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
            this._config.ChanceGrowth = growthChance;
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
            this._config.ChanceGrowth = growthChance;
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
                tree.currentTileLocation + new Vector2(-1, 0),
                tree.treeType.Value,
                Tree.treeStage
            );

            // Act, Assert
            return this.UpdateAndCheckHasGrown(tree, expectGrowth);
        }


        // ========== Shade is cast from the right positions ===========================================================

        private ITraversable BuildTest_ShadePositions()
        {
            ICasedTestBuilder<(Vector2 Offset, bool ExpectGrowth)> testBuilder =
                this._factory.CreateCasedTestBuilder<(Vector2, bool)>();

            testBuilder.Key = "shade_positions";
            testBuilder.TestMethod = this.Test_ShadePositions;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args => StringUtils.NormalizeNegatives($"x_{args.Offset.X}_y_{args.Offset.Y}");

            var shadeRadius = 1;
            for (int x = -(shadeRadius + 1); x <= shadeRadius + 1; x++)
            for (int y = -(shadeRadius + 1); y <= shadeRadius + 1; y++)
            {
                if (x == 0 && y == 0) continue;
                bool isShaded = Math.Abs(x) <= shadeRadius && Math.Abs(y) <= shadeRadius;
                testBuilder.AddCases((Offset: new Vector2(x, y), ExpectGrowth: !isShaded));
            }

            return testBuilder.Build();
        }


        private ITestResult Test_ShadePositions((Vector2 Offset, bool ExpectGrowth) args)
        {
            (Vector2 offset, bool expectGrowth) = args;

            // Arrange
            int initStage = Tree.seedStage;

            this._config.GrowthRoller = () => true;
            this._config.MaxShadedGrowthStage = initStage;

            Tree tree = Utilities.TreeUtils.GetFarmTreeLonely(initStage);
            Utilities.TreeUtils.PlantTree(
                tree.currentLocation,
                tree.currentTileLocation + offset,
                tree.treeType.Value,
                Tree.treeStage
            );

            // Act, Assert
            return this.UpdateAndCheckHasGrown(tree, expectGrowth);
        }


        // ========== Growth shading works as expected with mixed sources ==============================================


        private ITraversable BuildTest_ShadeSources()
        {
            ICasedTestBuilder<(bool Shading, bool Stump, bool NonShading, bool ExpectGrowth)> testBuilder =
                this._factory.CreateCasedTestBuilder<(bool, bool, bool, bool)>();

            testBuilder.Key = "shade_mixed";
            testBuilder.TestMethod = this.Test_ShadeSources;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args =>
            {
                var sources = new List<string>();
                if (args.Shading) sources.Add("shade");
                if (args.Stump) sources.Add("stump");
                if (args.NonShading) sources.Add("seed");
                return sources.Any() ? string.Join("_", sources) : "none";
            };

            foreach (bool shading in new[] {false, true})
            foreach (bool stump in new[] {false, true})
            foreach (bool nonShading in new[] {false, true})
            {
                testBuilder.AddCases((Shading: shading, Stump: stump, NonShading: nonShading, ExpectGrowth: !shading));
            }

            return testBuilder.Build();
        }


        private ITestResult Test_ShadeSources((bool Shading, bool NonShading, bool Stump, bool ExpectGrowth) args)
        {
            (bool shading, bool nonShading, bool stump, bool expectGrowth) = args;

            // Arrange
            int initStage = Tree.saplingStage;

            this._config.GrowthRoller = () => true;
            this._config.MaxShadedGrowthStage = initStage;

            Tree tree = Utilities.TreeUtils.GetFarmTreeLonely(initStage);

            if (shading)
            {
                Utilities.TreeUtils.PlantTree(
                    tree.currentLocation,
                    tree.currentTileLocation + new Vector2(-1, -1),
                    tree.treeType.Value,
                    Tree.treeStage
                );
            }

            if (nonShading)
            {
                Utilities.TreeUtils.PlantTree(
                        tree.currentLocation,
                        tree.currentTileLocation + new Vector2(-1, 1),
                        tree.treeType.Value,
                        Tree.treeStage
                    )
                    .growthStage.Value = Tree.seedStage;
            }

            if (stump)
            {
                Utilities.TreeUtils.PlantTree(
                        tree.currentLocation,
                        tree.currentTileLocation + new Vector2(-1, 0),
                        tree.treeType.Value,
                        Tree.treeStage
                    )
                    .stump.Value = true;
            }

            // Act, Assert
            return this.UpdateAndCheckHasGrown(tree, expectGrowth);
        }


        // ========== Instant growth ===================================================================================

        private ITraversable BuildTest_InstantGrowthObeysConfig()
        {
            ICasedTestBuilder<(int InitialStage, int NextStage)> testBuilder =
                this._factory.CreateCasedTestBuilder<(int, int)>();

            testBuilder.Key = "instant_growth";
            testBuilder.TestMethod = this.Test_InstantGrowthObeysConfig;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args => $"{args.InitialStage}";
            testBuilder.AddCases(
                (InitialStage: Tree.seedStage, NextStage: Tree.treeStage),
                (InitialStage: Tree.sproutStage, NextStage: Tree.treeStage),
                (InitialStage: Tree.saplingStage, NextStage: Tree.treeStage),
                (InitialStage: Tree.bushStage, NextStage: Tree.treeStage),
                (InitialStage: Tree.bushStage + 1, NextStage: Tree.treeStage),
                (InitialStage: Tree.treeStage, NextStage: Tree.treeStage)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_InstantGrowthObeysConfig((int, int) args)
        {
            (int initialStage, int nextStage) = args;

            // Arrange
            this._config.GrowthRoller = () => true;
            this._config.DoGrowInstantly = true;

            // Act, Assert
            return this.UpdateAndCheckHasGrown(Utilities.TreeUtils.GetFarmTreeLonely(initialStage), nextStage);
        }


        // ========== Instant growth - shade ===========================================================================

        private ITraversable BuildTest_InstantGrowthShaded()
        {
            ICasedTestBuilder<(int MaxShadedStage, int ExpectedStage)> testBuilder =
                this._factory.CreateCasedTestBuilder<(int, int)>();

            testBuilder.Key = "instant_shaded";
            testBuilder.TestMethod = this.Test_InstantGrowthShaded;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args => $"{args.MaxShadedStage}";
            testBuilder.AddCases(
                (MaxShadedStage: Tree.seedStage, ExpectedStage: Tree.seedStage),
                (MaxShadedStage: Tree.sproutStage, ExpectedStage: Tree.sproutStage),
                (MaxShadedStage: Tree.saplingStage, ExpectedStage: Tree.saplingStage),
                (MaxShadedStage: Tree.bushStage, ExpectedStage: Tree.bushStage),
                (MaxShadedStage: Tree.bushStage + 1, ExpectedStage: Tree.bushStage + 1),
                (MaxShadedStage: Tree.treeStage, ExpectedStage: Tree.treeStage)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_InstantGrowthShaded((int, int) args)
        {
            (int maxShadedStage, int expectedStage) = args;

            // Arrange
            this._config.GrowthRoller = () => true;
            this._config.DoGrowInstantly = true;
            this._config.MaxShadedGrowthStage = maxShadedStage;

            Tree tree = Utilities.TreeUtils.GetFarmTreeLonely(Tree.seedStage);
            Utilities.TreeUtils.PlantTree(
                tree.currentLocation,
                tree.currentTileLocation + new Vector2(-1, 0),
                tree.treeType.Value,
                Tree.treeStage
            );

            // Act, Assert
            return this.UpdateAndCheckHasGrown(tree, expectedStage);
        }
    }
}