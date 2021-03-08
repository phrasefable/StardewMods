using System.Collections.Generic;
using System.Linq;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Utilities;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

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
            ITestFixtureBuilder builder = _factory.CreateFixtureBuilder();
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
            tool.DoFunction(tree.currentLocation, -100, -100, -1, Game1.player);

            // Tree.performToolAction returns true if the tree is destroyed
            return tree.performToolAction(tool, 0, tree.currentTileLocation, tree.currentLocation) == expectEffect
                ? this._factory.BuildTestResult(Status.Pass)
                : this._factory.BuildTestResult(
                    Status.Fail,
                    $"Expected {(expectEffect ? "" : "no ")}effect."
                );
        }


        private Tool MakeTool(byte type)
        {
            return ToolFactory.getToolFromDescription(type, Tool.iridium);
        }


        // =============================================================================================================

        private ITraversable BuildTest_MeleeByStageAndConfig()
        {
            ICasedTestBuilder<(int Stage, bool ProtectFromMelee, bool ExpectAction)> testBuilder =
                this._factory.CreateCasedTestBuilder<(int Stage, bool ConfigValue, bool ExpectAction)>();

            testBuilder.Key = "melee";
            testBuilder.TestMethod = this.Test_MeleeByStageAndConfig;
            testBuilder.Delay = Delay.Tick;
            testBuilder.KeyGenerator = args => $"stage_{args.Stage}_with_config_{args.ProtectFromMelee}";

            testBuilder.AddCases(
                (Stage: Tree.seedStage, ProtectFromMelee: false, ExpectAction: false),
                (Stage: Tree.sproutStage, ProtectFromMelee: false, ExpectAction: true),
                (Stage: Tree.saplingStage, ProtectFromMelee: false, ExpectAction: true),
                (Stage: Tree.bushStage, ProtectFromMelee: false, ExpectAction: false),
                (Stage: Tree.bushStage + 1, ProtectFromMelee: false, ExpectAction: false),
                (Stage: Tree.treeStage, ProtectFromMelee: false, ExpectAction: false)
            );
            testBuilder.AddCases(
                (Stage: Tree.seedStage, ProtectFromMelee: true, ExpectAction: false),
                (Stage: Tree.sproutStage, ProtectFromMelee: true, ExpectAction: false),
                (Stage: Tree.saplingStage, ProtectFromMelee: true, ExpectAction: false),
                (Stage: Tree.bushStage, ProtectFromMelee: true, ExpectAction: false),
                (Stage: Tree.bushStage + 1, ProtectFromMelee: true, ExpectAction: false),
                (Stage: Tree.treeStage, ProtectFromMelee: true, ExpectAction: false)
            );

            return testBuilder.Build();
        }


        private ITestResult Test_MeleeByStageAndConfig((int Stage, bool ProtectFromMelee, bool ExpectAction) args)
        {
            (int stage, bool protectFromMelee, bool expectAction) = args;

            // Arrange
            Tree tree = Utilities.TreeUtils.GetFarmTreeLonely(stage);
            this._config.DoMeleeWeaponsDestroySeedlings = !protectFromMelee;
            Tool tool = this.MakeTool(ToolFactory.meleeWeapon);

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
                $"{args.Tool.BaseName}_stage_{args.Stage}_with_config_{args.ProtectFromMelee}";

            var tools = new Dictionary<Tool, int[]>
            {
                {this.MakeTool(ToolFactory.axe), Utilities.TreeUtils.Stages},
                {this.MakeTool(ToolFactory.pickAxe), new[] {Tree.seedStage, Tree.sproutStage, Tree.saplingStage}},
                {this.MakeTool(ToolFactory.hoe), new[] {Tree.seedStage, Tree.sproutStage, Tree.saplingStage}}
            };

            foreach (Tool tool in tools.Keys)
            foreach (bool protectFromMelee in new[] {false, true})
            foreach (int stage in Utilities.TreeUtils.Stages)
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
            Tree tree = Utilities.TreeUtils.GetFarmTreeLonely(stage);
            this._config.DoMeleeWeaponsDestroySeedlings = !protectFromMelee;

            // Assert
            return this.CheckIfToolAffectsTree(tree, tool, expectAction);
        }
    }
}
