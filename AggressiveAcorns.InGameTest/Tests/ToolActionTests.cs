using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Utilities;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Tests
{
    public class ToolActionTests
    {
        private readonly ITestDefinitionFactory _factory;


        private MutableConfigAdaptor _config;


        public ToolActionTests(ITestDefinitionFactory factory)
        {
            this._factory = factory;
        }


        public ITraversable Build()
        {
            ITestFixtureBuilder builder = this._factory.CreateFixtureBuilder();
            builder.Key = "tools";
            builder.AddCondition(this._factory.Conditions.WorldReady);

            builder.BeforeEach = () =>
            {
                this._config = new MutableConfigAdaptor();
                AggressiveAcorns.Config = this._config;
            };

            builder.Delay = Delay.Tick;

            builder.AddChild(this.BuildTest_MeleeByStageAndConfig());
            builder.AddChild(this.BuildTest_NonMeleeByStage());

            return builder.Build();
        }


        private ITestResult CheckIfToolAffectsTree(Tree tree, Tool tool, bool expectEffect)
        {
            // Set to low health to ensure hit will destroy
            tree.health.Value = 1;
            if (tree.growthStage.Value == Tree.treeStage) tree.stump.Value = true;

            // Need to take a practice swing using proper methods to init some private fields on tool
            tool.DoFunction(tree.Location, -100, -100, -1, Game1.player);

            // Tree.performToolAction returns true if the tree is destroyed
            return tree.performToolAction(tool, 0, tree.Tile) == expectEffect
                ? this._factory.BuildTestResult(Status.Pass)
                : this._factory.BuildTestResult(
                    Status.Fail,
                    $"Expected {(expectEffect ? "an" : "no")} effect."
                );
        }


        // =============================================================================================================

        private ITraversable BuildTest_MeleeByStageAndConfig()
        {
            ICasedTestBuilder<(int Stage, Tool Tool, bool ProtectFromMelee, bool ExpectAction)> testBuilder =
                this._factory.CreateCasedTestBuilder<(int Stage, Tool Tool, bool ConfigValue, bool ExpectAction)>();

            testBuilder.Key = "melee";
            testBuilder.TestMethod = this.Test_MeleeByStageAndConfig;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args => $"{args.Tool.ItemId}_stage_{args.Stage}_with_config_{args.ProtectFromMelee}";

            var tool = (Tool) ItemRegistry.Create("(W)66");

            testBuilder.AddCases(
                (Stage: Tree.seedStage, Tool: tool, ProtectFromMelee: false, ExpectAction: true),//false
                (Stage: Tree.sproutStage, Tool: tool, ProtectFromMelee: false, ExpectAction: true),
                (Stage: Tree.saplingStage, Tool: tool, ProtectFromMelee: false, ExpectAction: true),
                (Stage: Tree.bushStage, Tool: tool, ProtectFromMelee: false, ExpectAction: false),
                (Stage: Tree.bushStage + 1, Tool: tool, ProtectFromMelee: false, ExpectAction: false),
                (Stage: Tree.treeStage, Tool: tool, ProtectFromMelee: false, ExpectAction: false)
            );
            testBuilder.AddCases(
                (Stage: Tree.seedStage, Tool: tool, ProtectFromMelee: true, ExpectAction: false),
                (Stage: Tree.sproutStage, Tool: tool, ProtectFromMelee: true, ExpectAction: false),
                (Stage: Tree.saplingStage, Tool: tool, ProtectFromMelee: true, ExpectAction: false),
                (Stage: Tree.bushStage, Tool: tool, ProtectFromMelee: true, ExpectAction: false),
                (Stage: Tree.bushStage + 1, Tool: tool, ProtectFromMelee: true, ExpectAction: false),
                (Stage: Tree.treeStage, Tool: tool, ProtectFromMelee: true, ExpectAction: false)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_MeleeByStageAndConfig((int Stage, Tool Tool, bool ProtectFromMelee, bool ExpectAction) args)
        {
            (int stage, Tool tool, bool protectFromMelee, bool expectAction) = args;

            // Arrange
            Tree tree = TreeUtils.GetFarmTreeLonely(stage);
            this._config.DoMeleeWeaponsDestroySeedlings = !protectFromMelee;

            // Assert
            return this.CheckIfToolAffectsTree(tree, tool, expectAction);
        }


        // =============================================================================================================

        private ITraversable BuildTest_NonMeleeByStage()
        {
            ICasedTestBuilder<(int Stage, Tool Tool, bool ProtectFromMelee, bool ExpectAction)> testBuilder =
                this._factory.CreateCasedTestBuilder<(int, Tool, bool, bool)>();

            testBuilder.Key = "others";
            testBuilder.TestMethod = this.Test_PassableByHealth;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args =>
                $"{args.Tool.ItemId}_stage_{args.Stage}_with_config_{args.ProtectFromMelee}";

            var tools = new Dictionary<Tool, int[]>
            {
                {ItemRegistry.Create<Tool>("(T)IridiumAxe"), TreeUtils.Stages},
                {ItemRegistry.Create<Tool>("(T)IridiumPickaxe"), new[] {Tree.seedStage, Tree.sproutStage, Tree.saplingStage}},
                {ItemRegistry.Create<Tool>("(T)IridiumHoe"), new[] {Tree.seedStage, Tree.sproutStage, Tree.saplingStage}}
            };

            foreach (Tool tool in tools.Keys)
                foreach (bool protectFromMelee in new[] { false, true })
                    foreach (int stage in TreeUtils.Stages)
                    {
                        testBuilder.AddCases(
                            (
                                Stage: stage,
                                Tool: tool,
                                ProtectFromMelee: protectFromMelee,
                                ExpectAction: tools[tool].Contains(stage)
                            )
                        );
                    }

            return testBuilder.Build();
        }


        private ITestResult Test_PassableByHealth((int Stage, Tool Tool, bool ProtectFromMelee, bool ExpectAction) args)
        {
            (int stage, Tool tool, bool protectFromMelee, bool expectAction) = args;

            // Arrange
            Tree tree = TreeUtils.GetFarmTreeLonely(stage);
            this._config.DoMeleeWeaponsDestroySeedlings = !protectFromMelee;

            // Assert
            return this.CheckIfToolAffectsTree(tree, tool, expectAction);
        }
    }
}
