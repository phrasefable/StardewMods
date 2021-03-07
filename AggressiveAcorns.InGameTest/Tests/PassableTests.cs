using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Utilities;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Tests
{
    public class PassableTests
    {
        private readonly ITestDefinitionFactory _factory;

        private MutableConfigAdaptor _config;


        public PassableTests(ITestDefinitionFactory factory)
        {
            this._factory = factory;
        }


        public ITraversable Build()
        {
            ITestFixtureBuilder builder = _factory.CreateFixtureBuilder();
            builder.Key = "passable";
            builder.AddCondition(this._factory.Conditions.WorldReady);

            builder.BeforeEach = () =>
            {
                this._config = new MutableConfigAdaptor();
                AggressiveAcorns.Config = this._config;
            };

            builder.Delay = Delay.Tick;

            builder.AddChild(this.BuildTest_PassableByStage());
            builder.AddChild(this.BuildTest_PassableByHealth());

            return builder.Build();
        }


        private ITestResult CheckWhetherPassable(Tree tree, bool expectPassable)
        {
            return tree.isPassable(Game1.player) == expectPassable
                ? this._factory.BuildTestResult(Status.Pass)
                : this._factory.BuildTestResult(
                    Status.Fail,
                    $"Expected {(expectPassable ? "" : "not ")}passable."
                );
        }


        // =============================================================================================================

        private ITraversable BuildTest_PassableByStage()
        {
            ICasedTestBuilder<(int Stage, int ConfigValue, bool ExpectPassable)> testBuilder =
                this._factory.CreateCasedTestBuilder<(int Stage, int ConfigValue, bool ExpectPassable)>();

            testBuilder.Key = "stage";
            testBuilder.TestMethod = this.Test_PassableByStage;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args => $"stage_{args.Stage}_with_max_{args.ConfigValue}";

            foreach (int configValue in Utilities.TreeUtils.Stages)
            foreach (int treeStage in Utilities.TreeUtils.Stages)
            {
                testBuilder.AddCases(
                    (Stage: treeStage, ConfigValue: configValue, ExpectPassable: treeStage <= configValue)
                );
            }

            return testBuilder.Build();
        }


        private ITestResult Test_PassableByStage((int Stage, int ConfigValue, bool ExpectPassable) args)
        {
            (int stage, int configValue, bool expectPassable) = args;

            // Arrange
            Tree tree = Utilities.TreeUtils.GetFarmTreeLonely(stage);
            this._config.MaxPassableGrowthStage = configValue;

            // Assert
            return this.CheckWhetherPassable(tree, expectPassable);
        }


        // =============================================================================================================

        private ITraversable BuildTest_PassableByHealth()
        {
            ICasedTestBuilder<(int Health, bool ExpectPassable)> testBuilder =
                this._factory.CreateCasedTestBuilder<(int Stage, bool ExpectPassable)>();

            testBuilder.Key = "health";
            testBuilder.TestMethod = this.Test_PassableByHealth;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args => StringUtils.NormalizeNegatives(args.Health.ToString());

            testBuilder.AddCases(
                (Health: -100, ExpectPassable: true),
                (Health: -99, ExpectPassable: true),
                (Health: -98, ExpectPassable: false),
                (Health: 0, ExpectPassable: false),
                (Health: 10, ExpectPassable: false)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_PassableByHealth((int Health, bool ExpectPassable) args)
        {
            (int health, bool expectPassable) = args;

            // Arrange
            this._config.MaxPassableGrowthStage = -1;

            Tree tree = Utilities.TreeUtils.GetFarmTreeLonely();
            tree.health.Value = health;

            // Assert
            return this.CheckWhetherPassable(tree, expectPassable);
        }
    }
}
