using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Tests
{
    internal partial class GrowthTests
    {
        // ========== Mahogany grows, Overriding random ================================================================

        private ITraversable BuildTest_MahoganyGrows()
        {
            ICasedTestBuilder<(double GrowthChance, bool ForcedValue)> testBuilder =
                this._factory.CreateCasedTestBuilder<(double, bool)>();

            testBuilder.Key = "mahogany_grows";
            testBuilder.TestMethod = this.Test_MahoganyGrows;
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


        private ITestResult Test_MahoganyGrows((double, bool) args)
        {
            (double growthChance, bool forcedValue) = args;

            // Arrange
            this._config.ChanceGrowth = 0.0;
            this._config.GrowthRoller = () => false;
            this._config.ChanceGrowthMahogany = growthChance;
            this._config.GrowthMahoganyRoller = () => forcedValue;
            Tree tree = Utilities.TreeUtils.GetFarmTreeLonely(Tree.saplingStage, Tree.mahoganyTree);

            // Act, Assert
            return this.UpdateAndCheckHasGrown(tree, forcedValue);
        }


        // ========== Mahogany grows, varying config chance ============================================================

        private ITraversable BuildTest_MahoganyGrowthChanceObeysConfig()
        {
            ICasedTestBuilder<(double GrowthChance, bool ExpectGrowth)> testBuilder =
                this._factory.CreateCasedTestBuilder<(double, bool)>();

            testBuilder.Key = "mahogany_config";
            testBuilder.TestMethod = this.Test_MahoganyGrowthChanceObeysConfig;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args => $"{args.GrowthChance}";
            testBuilder.AddCases(
                (GrowthChance: 0.0, ExpectGrowth: false),
                (GrowthChance: 1.0, ExpectGrowth: true)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_MahoganyGrowthChanceObeysConfig((double, bool) args)
        {
            (double growthChance, bool expectGrowth) = args;

            // Arrange
            this._config.ChanceGrowth = 0.0;
            this._config.GrowthRoller = () => false;
            this._config.ChanceGrowthMahogany = growthChance;
            Tree tree = Utilities.TreeUtils.GetFarmTreeLonely(Tree.saplingStage, Tree.mahoganyTree);

            // Act, Assert
            return this.UpdateAndCheckHasGrown(tree, expectGrowth);
        }


        // ========== Fertilized mahogany grows, Overriding random =====================================================

        private ITraversable BuildTest_FertilizedMahoganyGrows()
        {
            ICasedTestBuilder<(double GrowthChance, bool ForcedValue)> testBuilder =
                this._factory.CreateCasedTestBuilder<(double, bool)>();

            testBuilder.Key = "f_mahogany_grows";
            testBuilder.TestMethod = this.Test_FertilizedMahoganyGrows;
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


        private ITestResult Test_FertilizedMahoganyGrows((double, bool) args)
        {
            (double growthChance, bool forcedValue) = args;

            // Arrange
            this._config.ChanceGrowth = 0.0;
            this._config.GrowthRoller = () => false;
            this._config.ChanceGrowthMahogany = 0.0;
            this._config.GrowthMahoganyRoller = () => false;
            this._config.ChanceGrowthMahoganyFertilized = growthChance;
            this._config.GrowthMahoganyFertilizedRoller = () => forcedValue;
            Tree tree = Utilities.TreeUtils.GetFarmTreeLonely(Tree.saplingStage, Tree.mahoganyTree);
            tree.fertilized.Value = true;

            // Act, Assert
            return this.UpdateAndCheckHasGrown(tree, forcedValue);
        }


        // ========== Mahogany grows, varying config chance ============================================================

        private ITraversable BuildTest_FertilizedMahoganyGrowthChanceObeysConfig()
        {
            ICasedTestBuilder<(double GrowthChance, bool ExpectGrowth)> testBuilder =
                this._factory.CreateCasedTestBuilder<(double, bool)>();

            testBuilder.Key = "f_mahogany_config";
            testBuilder.TestMethod = this.Test_FertilizedMahoganyGrowthChanceObeysConfig;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args => $"{args.GrowthChance}";
            testBuilder.AddCases(
                (GrowthChance: 0.0, ExpectGrowth: false),
                (GrowthChance: 1.0, ExpectGrowth: true)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_FertilizedMahoganyGrowthChanceObeysConfig((double, bool) args)
        {
            (double growthChance, bool expectGrowth) = args;

            // Arrange
            this._config.ChanceGrowth = 0.0;
            this._config.GrowthRoller = () => false;
            this._config.ChanceGrowthMahogany = 0.0;
            this._config.GrowthMahoganyRoller = () => false;
            this._config.ChanceGrowthMahoganyFertilized = growthChance;
            Tree tree = Utilities.TreeUtils.GetFarmTreeLonely(Tree.saplingStage, Tree.mahoganyTree);
            tree.fertilized.Value = true;

            // Act, Assert
            return this.UpdateAndCheckHasGrown(tree, expectGrowth);
        }
    }
}
