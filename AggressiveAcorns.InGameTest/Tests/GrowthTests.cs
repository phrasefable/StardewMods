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
                TestTree.ResetData();
            };
            fixtureBuilder.AfterAllDelay = Delay.Second;

            fixtureBuilder.BeforeEach = () =>
            {
                AggressiveAcorns.Config = ModConfigUtils.GetVanillaDefaults();
                TestTree.ResetData();
            };

            // fixtureBuilder.Delay = Delay.Tick;

            fixtureBuilder.AddChild(this.BuildTest_GrowthChanceObeysConfig());
            fixtureBuilder.AddChild(this.BuildTest_GrowthChanceFertilizedObeysConfig());

            fixtureBuilder.AddChild(this.BuildTest_GrowthChanceVanilla());
            fixtureBuilder.AddChild(this.BuildTest_GrowthChanceFertilizedVanilla());

            fixtureBuilder.AddChild(this.BuildTest_StageSequence());

            fixtureBuilder.AddChild(this.BuildTest_GrowthChanceOverrideObeysConfig());
            fixtureBuilder.AddChild(this.BuildTest_GrowthChanceOverrideContained());
            fixtureBuilder.AddChild(this.BuildTest_GrowthChanceOverrideRevertToVanilla());

            fixtureBuilder.AddChild(this.BuildTest_GrowthChanceFertilizedOverrideObeysConfig());
            fixtureBuilder.AddChild(this.BuildTest_GrowthChanceFertilizedOverrideContained());
            fixtureBuilder.AddChild(this.BuildTest_GrowthChanceFertilizedOverrideRevertToVanilla());

            //fixtureBuilder.AddChild(this.BuildTest_ShadeObeysConfig());
            //fixtureBuilder.AddChild(this.BuildTest_ShadePositions());
            //fixtureBuilder.AddChild(this.BuildTest_ShadeSources());
            //fixtureBuilder.AddChild(this.BuildTest_InstantGrowthObeysConfig());
            //fixtureBuilder.AddChild(this.BuildTest_InstantGrowthShaded());
            //fixtureBuilder.AddChild(this.BuildFixture_WinterGrowth());

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
            tree.dayUpdate();

            // Assert
            return tree.growthStage.Value == expectedStage
                ? this._factory.BuildTestResult(Status.Pass)
                : this._factory.BuildTestResult(
                    Status.Fail,
                    $"Expected stage {expectedStage}, got {tree.growthStage.Value}."
                );
        }


        // ========== Grows, varying config chance =====================================================================

        private ITraversable BuildTest_GrowthChanceObeysConfig()
        {
            ICasedTestBuilder<(int GrowthChance, bool ExpectGrowth)> testBuilder =
                this._factory.CreateCasedTestBuilder<(int, bool)>();

            testBuilder.Key = "config_chance";
            testBuilder.TestMethod = this.Test_GrowthChanceObeysConfig;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args => $"chance_{args.GrowthChance}";
            testBuilder.AddCases(
                (GrowthChance: 0, ExpectGrowth: false),
                (GrowthChance: 100, ExpectGrowth: true)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_GrowthChanceObeysConfig((int, bool) args)
        {
            (int growthChance, bool expectGrowth) = args;

            // Arrange
            AggressiveAcorns.Config.ChanceGrowth = growthChance;
            TestTree.TreeData.GrowthChance = growthChance == 0 ? 1.0f : 0.0f;
            var tree = TreeUtils.GetFarmTreeLonely(Tree.saplingStage, TestTree.Id);

            // Act, Assert
            return this.UpdateAndCheckHasGrown(tree, expectGrowth);
        }


        // ========== Grows when fertilized, varying config chance =====================================================

        private ITraversable BuildTest_GrowthChanceFertilizedObeysConfig()
        {
            ICasedTestBuilder<(int GrowthChance, bool ExpectGrowth)> testBuilder =
                this._factory.CreateCasedTestBuilder<(int, bool)>();

            testBuilder.Key = "config_chance_fertilized";
            testBuilder.TestMethod = this.Test_GrowthChanceFertilizedObeysConfig;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args => $"chance_{args.GrowthChance}";
            testBuilder.AddCases(
                (GrowthChance: 0, ExpectGrowth: false),
                (GrowthChance: 100, ExpectGrowth: true)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_GrowthChanceFertilizedObeysConfig((int, bool) args)
        {
            (int growthChance, bool expectGrowth) = args;

            // Arrange
            AggressiveAcorns.Config.ChanceGrowthFertilized = growthChance;
            TestTree.TreeData.FertilizedGrowthChance = growthChance == 0 ? 1.0f : 0.0f;
            Tree tree = TreeUtils.GetFarmTreeLonely(Tree.saplingStage, TestTree.Id);
            tree.fertilized.Value = true;


            // Act, Assert
            return this.UpdateAndCheckHasGrown(tree, expectGrowth);
        }


        // ========== Grows, varying config chance =====================================================================

        private ITraversable BuildTest_GrowthChanceVanilla()
        {
            ICasedTestBuilder<bool> testBuilder =
                this._factory.CreateCasedTestBuilder<bool>();

            testBuilder.Key = "config_vanilla";
            testBuilder.TestMethod = this.Test_GrowthChanceVanilla;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = expectGrowth => $"shouldgrow_{expectGrowth}";
            testBuilder.AddCases(
                false,
                true
            );

            return testBuilder.Build();
        }


        private ITestResult Test_GrowthChanceVanilla(bool expectGrowth)
        {
            // Arrange
            AggressiveAcorns.Config.ChanceGrowth = -1;
            TestTree.TreeData.GrowthChance = expectGrowth ? 1.0f : 0.0f;
            var tree = TreeUtils.GetFarmTreeLonely(Tree.saplingStage, TestTree.Id);

            // Act, Assert
            return this.UpdateAndCheckHasGrown(tree, expectGrowth);
        }


        // ========== Grows when fertilized, varying config chance =====================================================

        private ITraversable BuildTest_GrowthChanceFertilizedVanilla()
        {
            ICasedTestBuilder<bool> testBuilder =
                this._factory.CreateCasedTestBuilder<bool>();

            testBuilder.Key = "config_vanilla_fertilized";
            testBuilder.TestMethod = this.Test_GrowthChanceFertilizedVanilla;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = expectGrowth => $"shouldgrow_{expectGrowth}";
            testBuilder.AddCases(
                false,
                true
            );

            return testBuilder.Build();
        }


        private ITestResult Test_GrowthChanceFertilizedVanilla(bool expectGrowth)
        {
            // Arrange
            AggressiveAcorns.Config.ChanceGrowthFertilized = -1;
            TestTree.TreeData.FertilizedGrowthChance = expectGrowth ? 1.0f : 0.0f;
            Tree tree = TreeUtils.GetFarmTreeLonely(Tree.saplingStage, TestTree.Id);
            tree.fertilized.Value = true;

            // Act, Assert
            return this.UpdateAndCheckHasGrown(tree, expectGrowth);
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
                (InitialStage: Tree.treeStage, NextStage: Tree.treeStage + 1),
                (InitialStage: Tree.stageForMossGrowth, NextStage: AggressiveAcorns.MaxGrowthStage),
                (InitialStage: AggressiveAcorns.MaxGrowthStage, NextStage: AggressiveAcorns.MaxGrowthStage)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_StageSequence((int, int) args)
        {
            (int initialStage, int nextStage) = args;

            // Arrange
            AggressiveAcorns.Config.ChanceGrowth = 100;

            // Act, Assert
            return this.UpdateAndCheckHasGrown(TreeUtils.GetFarmTreeLonely(initialStage, TestTree.Id), nextStage);
        }


        // ========== Growth override works ============================================================================

        private ITraversable BuildTest_GrowthChanceOverrideObeysConfig()
        {
            ICasedTestBuilder<(int BaseChance, int OverridingChance)> testBuilder =
                this._factory.CreateCasedTestBuilder<(int, int)>();

            testBuilder.Key = "config_chance_override";
            testBuilder.TestMethod = this.Test_GrowthChanceOverrideObeysConfig;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args => $"chance_{args.BaseChance}_overridden_{args.OverridingChance}".NormalizeNegatives();
            testBuilder.AddCases(
                (BaseChance: -1, OverridingChance: 0),
                (BaseChance: -1, OverridingChance: 100),
                (BaseChance: 0, OverridingChance: 0),
                (BaseChance: 0, OverridingChance: 100),
                (BaseChance: 100, OverridingChance: 0),
                (BaseChance: 100, OverridingChance: 100)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_GrowthChanceOverrideObeysConfig((int, int) args)
        {
            (int baseChance, int overriddenChance) = args;
            bool expectGrowth = overriddenChance == 100;

            // Arrange
            AggressiveAcorns.Config.ChanceGrowth = baseChance;
            AggressiveAcorns.Config.ChanceGrowth_Overrides[TestTree.Id] = overriddenChance;
            TestTree.TreeData.GrowthChance = expectGrowth ? 0.0f : 1.0f;
            var tree = TreeUtils.GetFarmTreeLonely(Tree.saplingStage, TestTree.Id);

            // Act, Assert
            return this.UpdateAndCheckHasGrown(tree, expectGrowth);
        }


        // ========== Growth override only applies to selected type ====================================================

        private ITraversable BuildTest_GrowthChanceOverrideContained()
        {
            ICasedTestBuilder<(int BaseChance, int OverridingChance)> testBuilder =
                this._factory.CreateCasedTestBuilder<(int, int)>();

            testBuilder.Key = "chance_override_contained";
            testBuilder.TestMethod = this.Test_GrowthChanceOverrideContained;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args => $"chance_{args.BaseChance}_overridden_{args.OverridingChance}".NormalizeNegatives();
            testBuilder.AddCases(
                (BaseChance: 0, OverridingChance: -1),
                (BaseChance: 0, OverridingChance: 0),
                (BaseChance: 0, OverridingChance: 100),
                (BaseChance: 100, OverridingChance: -1),
                (BaseChance: 100, OverridingChance: 0),
                (BaseChance: 100, OverridingChance: 100)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_GrowthChanceOverrideContained((int, int) args)
        {
            (int baseChance, int overriddenChance) = args;
            bool expectGrowth = baseChance == 100;

            // Arrange
            AggressiveAcorns.Config.ChanceGrowth = baseChance;
            AggressiveAcorns.Config.ChanceGrowth_Overrides[Tree.bushyTree] = overriddenChance;
            var tree = TreeUtils.GetFarmTreeLonely(Tree.saplingStage, TestTree.Id);

            // Act, Assert
            return this.UpdateAndCheckHasGrown(tree, expectGrowth);
        }


        // ========== Growth override of -1 reverts that tree to vanilla ===============================================

        private ITraversable BuildTest_GrowthChanceOverrideRevertToVanilla()
        {
            ICasedTestBuilder<(bool VanillaDoesGrow, int OverriddenGrowthChance)> testBuilder =
                this._factory.CreateCasedTestBuilder<(bool, int)>();

            testBuilder.Key = "chance_override_reverts";
            testBuilder.TestMethod = this.Test_GrowthChanceOverrideRevertToVanilla;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args => $"chance_{args.OverriddenGrowthChance}_reverts_to_{(args.VanillaDoesGrow ? "growth" : "no_growth")}".NormalizeNegatives();
            testBuilder.AddCases(
                (VanillaDoesGrow: true, OverriddenGrowthChance: -1),
                (VanillaDoesGrow: true, OverriddenGrowthChance: 0),
                (VanillaDoesGrow: true, OverriddenGrowthChance: 100),
                (VanillaDoesGrow: false, OverriddenGrowthChance: -1),
                (VanillaDoesGrow: false, OverriddenGrowthChance: 0),
                (VanillaDoesGrow: false, OverriddenGrowthChance: 100)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_GrowthChanceOverrideRevertToVanilla((bool, int) args)
        {
            (bool vanillaDoesGrow, int overriddenGrowthChance) = args;

            //// Arrange
            TestTree.TreeData.GrowthChance = vanillaDoesGrow ? 1.0f : 0.0f;
            AggressiveAcorns.Config.ChanceGrowth = overriddenGrowthChance;
            AggressiveAcorns.Config.ChanceGrowth_Overrides[TestTree.Id] = -1;
            var tree = TreeUtils.GetFarmTreeLonely(Tree.saplingStage, TestTree.Id);

            // Act, Assert
            return this.UpdateAndCheckHasGrown(tree, vanillaDoesGrow);
        }


        // ========== Fertilized Growth override works =================================================================

        private ITraversable BuildTest_GrowthChanceFertilizedOverrideObeysConfig()
        {
            ICasedTestBuilder<(int BaseChance, int OverriddenChance)> testBuilder =
                this._factory.CreateCasedTestBuilder<(int, int)>();

            testBuilder.Key = "config_chance_fertilized_override";
            testBuilder.TestMethod = this.Test_GrowthChanceFertilizedOverrideObeysConfig;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args => $"chance_{args.BaseChance}_overridden_{args.OverriddenChance}".NormalizeNegatives();
            testBuilder.AddCases(
                (BaseChance: -1, OverriddenChance: 0),
                (BaseChance: -1, OverriddenChance: 100),
                (BaseChance: 0, OverriddenChance: 0),
                (BaseChance: 0, OverriddenChance: 100),
                (BaseChance: 100, OverriddenChance: 0),
                (BaseChance: 100, OverriddenChance: 100)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_GrowthChanceFertilizedOverrideObeysConfig((int, int) args)
        {
            (int baseChance, int overriddenChance) = args;
            bool expectGrowth = overriddenChance == 100;

            // Arrange
            AggressiveAcorns.Config.ChanceGrowthFertilized = baseChance;
            AggressiveAcorns.Config.ChanceGrowthFertilized_Overrides[TestTree.Id] = overriddenChance;
            TestTree.TreeData.FertilizedGrowthChance = expectGrowth ? 0.0f : 1.0f;
            var tree = TreeUtils.GetFarmTreeLonely(Tree.saplingStage, TestTree.Id);
            tree.fertilized.Value = true;

            // Act, Assert
            return this.UpdateAndCheckHasGrown(tree, expectGrowth);
        }


        // ========== Fertilized growth override only applies to selected type =========================================

        private ITraversable BuildTest_GrowthChanceFertilizedOverrideContained()
        {
            ICasedTestBuilder<(int BaseChance, int OverridingChance)> testBuilder =
                this._factory.CreateCasedTestBuilder<(int, int)>();

            testBuilder.Key = "chance_fertilized_override_contained";
            testBuilder.TestMethod = this.Test_GrowthChanceFertilizedOverrideContained;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args => $"chance_{args.BaseChance}_overridden_{args.OverridingChance}".NormalizeNegatives();
            testBuilder.AddCases(
                (BaseChance: 0, OverridingChance: -1),
                (BaseChance: 0, OverridingChance: 0),
                (BaseChance: 0, OverridingChance: 100),
                (BaseChance: 100, OverridingChance: -1),
                (BaseChance: 100, OverridingChance: 0),
                (BaseChance: 100, OverridingChance: 100)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_GrowthChanceFertilizedOverrideContained((int, int) args)
        {
            (int baseChance, int overriddenChance) = args;
            bool expectGrowth = baseChance == 100;

            // Arrange
            AggressiveAcorns.Config.ChanceGrowthFertilized = baseChance;
            AggressiveAcorns.Config.ChanceGrowthFertilized_Overrides[Tree.bushyTree] = overriddenChance;
            var tree = TreeUtils.GetFarmTreeLonely(Tree.saplingStage, TestTree.Id);
            tree.fertilized.Value = true;

            // Act, Assert
            return this.UpdateAndCheckHasGrown(tree, expectGrowth);
        }


        // ========== Fertilized growth override of -1 reverts that tree to vanilla ====================================

        private ITraversable BuildTest_GrowthChanceFertilizedOverrideRevertToVanilla()
        {
            ICasedTestBuilder<(bool VanillaDoesGrow, int OverriddenGrowthChance)> testBuilder =
                this._factory.CreateCasedTestBuilder<(bool, int)>();

            testBuilder.Key = "chance_fertilized_override_reverts";
            testBuilder.TestMethod = this.Test_GrowthChanceFertilizedOverrideRevertToVanilla;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args => $"chance_{args.OverriddenGrowthChance}_reverts_to_{(args.VanillaDoesGrow ? "growth" : "no_growth")}".NormalizeNegatives();
            testBuilder.AddCases(
                (VanillaDoesGrow: true, OverriddenGrowthChance: -1),
                (VanillaDoesGrow: true, OverriddenGrowthChance: 0),
                (VanillaDoesGrow: true, OverriddenGrowthChance: 100),
                (VanillaDoesGrow: false, OverriddenGrowthChance: -1),
                (VanillaDoesGrow: false, OverriddenGrowthChance: 0),
                (VanillaDoesGrow: false, OverriddenGrowthChance: 100)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_GrowthChanceFertilizedOverrideRevertToVanilla((bool, int) args)
        {
            (bool vanillaDoesGrow, int overriddenGrowthChance) = args;

            //// Arrange
            TestTree.TreeData.FertilizedGrowthChance = vanillaDoesGrow ? 1.0f : 0.0f;
            AggressiveAcorns.Config.ChanceGrowthFertilized = overriddenGrowthChance;
            AggressiveAcorns.Config.ChanceGrowthFertilized_Overrides[TestTree.Id] = -1;
            var tree = TreeUtils.GetFarmTreeLonely(Tree.saplingStage, TestTree.Id);
            tree.fertilized.Value = true;

            // Act, Assert
            return this.UpdateAndCheckHasGrown(tree, vanillaDoesGrow);
        }


        //// ========== Growth respects max shaded stage config option ===================================================

        //private ITraversable BuildTest_ShadeObeysConfig()
        //{
        //    ICasedTestBuilder<(int MaxShadedGrowthStage, int InitStage, bool ExpectGrowth)> testBuilder =
        //        this._factory.CreateCasedTestBuilder<(int, int, bool)>();

        //    testBuilder.Key = "shade_config";
        //    testBuilder.TestMethod = this.Test_ShadeObeysConfig;
        //    testBuilder.Delay = Delay.Tick;
        //    testBuilder.KeyGenerator = args => $"init_{args.InitStage}_max_{args.MaxShadedGrowthStage}";

        //    for (int maxShaded = Tree.seedStage; maxShaded <= Tree.treeStage; maxShaded++)
        //    {
        //        for (int initStage = Tree.seedStage; initStage <= Tree.treeStage; initStage++)
        //        {
        //            testBuilder.AddCases(
        //                (MaxShadedGrowthStage: maxShaded, InitStage: initStage, ExpectGrowth: initStage < maxShaded)
        //            );
        //        }
        //    }

        //    return testBuilder.Build();
        //}


        //private ITestResult Test_ShadeObeysConfig((int, int, bool) args)
        //{
        //    (int maxShadedGrowthStage, int initStage, bool expectGrowth) = args;

        //    // Arrange
        //    this._config.GrowthRoller = () => true;
        //    this._config.MaxShadedGrowthStage = maxShadedGrowthStage;

        //    Tree tree = TreeUtils.GetFarmTreeLonely(initStage);
        //    TreeUtils.PlantTree(
        //        tree.Location,
        //        tree.Tile + new Vector2(-1, 0),
        //        tree.treeType.Value,
        //        Tree.treeStage
        //    );

        //    // Act, Assert
        //    return this.UpdateAndCheckHasGrown(tree, expectGrowth);
        //}


        //// ========== Shade is cast from the right positions ===========================================================

        //private ITraversable BuildTest_ShadePositions()
        //{
        //    ICasedTestBuilder<(Vector2 Offset, bool ExpectGrowth)> testBuilder =
        //        this._factory.CreateCasedTestBuilder<(Vector2, bool)>();

        //    testBuilder.Key = "shade_positions";
        //    testBuilder.TestMethod = this.Test_ShadePositions;
        //    testBuilder.Delay = Delay.Tick;
        //    testBuilder.KeyGenerator = args => StringUtils.NormalizeNegatives($"x_{args.Offset.X}_y_{args.Offset.Y}");

        //    int shadeRadius = 1;
        //    for (int x = -(shadeRadius + 1); x <= shadeRadius + 1; x++)
        //        for (int y = -(shadeRadius + 1); y <= shadeRadius + 1; y++)
        //        {
        //            if (x == 0 && y == 0) continue;
        //            bool isShaded = Math.Abs(x) <= shadeRadius && Math.Abs(y) <= shadeRadius;
        //            testBuilder.AddCases((Offset: new Vector2(x, y), ExpectGrowth: !isShaded));
        //        }

        //    return testBuilder.Build();
        //}


        //private ITestResult Test_ShadePositions((Vector2 Offset, bool ExpectGrowth) args)
        //{
        //    (Vector2 offset, bool expectGrowth) = args;

        //    // Arrange
        //    int initStage = Tree.seedStage;

        //    this._config.GrowthRoller = () => true;
        //    this._config.MaxShadedGrowthStage = initStage;

        //    Tree tree = TreeUtils.GetFarmTreeLonely(initStage);
        //    TreeUtils.PlantTree(
        //        tree.Location,
        //        tree.Tile + offset,
        //        tree.treeType.Value,
        //        Tree.treeStage
        //    );

        //    // Act, Assert
        //    return this.UpdateAndCheckHasGrown(tree, expectGrowth);
        //}


        //// ========== Growth shading works as expected with mixed sources ==============================================


        //private ITraversable BuildTest_ShadeSources()
        //{
        //    ICasedTestBuilder<(bool Shading, bool Stump, bool NonShading, bool ExpectGrowth)> testBuilder =
        //        this._factory.CreateCasedTestBuilder<(bool, bool, bool, bool)>();

        //    testBuilder.Key = "shade_mixed";
        //    testBuilder.TestMethod = this.Test_ShadeSources;
        //    testBuilder.Delay = Delay.Tick;
        //    testBuilder.KeyGenerator = args =>
        //    {
        //        var sources = new List<string>();
        //        if (args.Shading) sources.Add("shade");
        //        if (args.Stump) sources.Add("stump");
        //        if (args.NonShading) sources.Add("seed");
        //        return sources.Any() ? string.Join("_", sources) : "none";
        //    };

        //    foreach (bool shading in new[] { false, true })
        //        foreach (bool stump in new[] { false, true })
        //            foreach (bool nonShading in new[] { false, true })
        //            {
        //                testBuilder.AddCases((Shading: shading, Stump: stump, NonShading: nonShading, ExpectGrowth: !shading));
        //            }

        //    return testBuilder.Build();
        //}


        //private ITestResult Test_ShadeSources((bool Shading, bool NonShading, bool Stump, bool ExpectGrowth) args)
        //{
        //    (bool shading, bool nonShading, bool stump, bool expectGrowth) = args;

        //    // Arrange
        //    int initStage = Tree.saplingStage;

        //    this._config.GrowthRoller = () => true;
        //    this._config.MaxShadedGrowthStage = initStage;

        //    Tree tree = TreeUtils.GetFarmTreeLonely(initStage);

        //    if (shading)
        //    {
        //        TreeUtils.PlantTree(
        //            tree.Location,
        //            tree.Tile + new Vector2(-1, -1),
        //            tree.treeType.Value,
        //            Tree.treeStage
        //        );
        //    }

        //    if (nonShading)
        //    {
        //        TreeUtils.PlantTree(
        //                tree.Location,
        //                tree.Tile + new Vector2(-1, 1),
        //                tree.treeType.Value,
        //                Tree.treeStage
        //            )
        //            .growthStage.Value = Tree.seedStage;
        //    }

        //    if (stump)
        //    {
        //        TreeUtils.PlantTree(
        //                tree.Location,
        //                tree.Tile + new Vector2(-1, 0),
        //                tree.treeType.Value,
        //                Tree.treeStage
        //            )
        //            .stump.Value = true;
        //    }

        //    // Act, Assert
        //    return this.UpdateAndCheckHasGrown(tree, expectGrowth);
        //}


        //// ========== Instant growth ===================================================================================

        //private ITraversable BuildTest_InstantGrowthObeysConfig()
        //{
        //    ICasedTestBuilder<(int InitialStage, int NextStage)> testBuilder =
        //        this._factory.CreateCasedTestBuilder<(int, int)>();

        //    testBuilder.Key = "instant_growth";
        //    testBuilder.TestMethod = this.Test_InstantGrowthObeysConfig;
        //    testBuilder.Delay = Delay.Tick;
        //    testBuilder.KeyGenerator = args => $"{args.InitialStage}";
        //    testBuilder.AddCases(
        //        (InitialStage: Tree.seedStage, NextStage: Tree.treeStage),
        //        (InitialStage: Tree.sproutStage, NextStage: Tree.treeStage),
        //        (InitialStage: Tree.saplingStage, NextStage: Tree.treeStage),
        //        (InitialStage: Tree.bushStage, NextStage: Tree.treeStage),
        //        (InitialStage: Tree.bushStage + 1, NextStage: Tree.treeStage),
        //        (InitialStage: Tree.treeStage, NextStage: Tree.treeStage)
        //    );

        //    return testBuilder.Build();
        //}


        //private ITestResult Test_InstantGrowthObeysConfig((int, int) args)
        //{
        //    (int initialStage, int nextStage) = args;

        //    // Arrange
        //    this._config.GrowthRoller = () => true;
        //    this._config.DoGrowInstantly = true;

        //    // Act, Assert
        //    return this.UpdateAndCheckHasGrown(TreeUtils.GetFarmTreeLonely(initialStage), nextStage);
        //}


        //// ========== Instant growth - shade ===========================================================================

        //private ITraversable BuildTest_InstantGrowthShaded()
        //{
        //    ICasedTestBuilder<(int MaxShadedStage, int ExpectedStage)> testBuilder =
        //        this._factory.CreateCasedTestBuilder<(int, int)>();

        //    testBuilder.Key = "instant_shaded";
        //    testBuilder.TestMethod = this.Test_InstantGrowthShaded;
        //    testBuilder.Delay = Delay.Tick;
        //    testBuilder.KeyGenerator = args => $"{args.MaxShadedStage}";
        //    testBuilder.AddCases(
        //        (MaxShadedStage: Tree.seedStage, ExpectedStage: Tree.seedStage),
        //        (MaxShadedStage: Tree.sproutStage, ExpectedStage: Tree.sproutStage),
        //        (MaxShadedStage: Tree.saplingStage, ExpectedStage: Tree.saplingStage),
        //        (MaxShadedStage: Tree.bushStage, ExpectedStage: Tree.bushStage),
        //        (MaxShadedStage: Tree.bushStage + 1, ExpectedStage: Tree.bushStage + 1),
        //        (MaxShadedStage: Tree.treeStage, ExpectedStage: Tree.treeStage)
        //    );

        //    return testBuilder.Build();
        //}


        //private ITestResult Test_InstantGrowthShaded((int, int) args)
        //{
        //    (int maxShadedStage, int expectedStage) = args;

        //    // Arrange
        //    this._config.GrowthRoller = () => true;
        //    this._config.DoGrowInstantly = true;
        //    this._config.MaxShadedGrowthStage = maxShadedStage;

        //    Tree tree = TreeUtils.GetFarmTreeLonely(Tree.seedStage);
        //    TreeUtils.PlantTree(
        //        tree.Location,
        //        tree.Tile + new Vector2(-1, 0),
        //        tree.treeType.Value,
        //        Tree.treeStage
        //    );

        //    // Act, Assert
        //    return this.UpdateAndCheckHasGrown(tree, expectedStage);
        //}
    }
}
