using FluentAssertions;
using NUnit.Framework;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns.Test
{
    [TestFixture]
    public class PassabilityTests
    {
        private AggressiveTree _tree;


        [SetUp]
        public void SetUp()
        {
            AggressiveAcorns.Config = new ConfigAdapter(new ModConfig());
            _tree = new AggressiveTree();
        }


        [TestCase(Tree.seedStage, true)]
        [TestCase(Tree.sproutStage, false)]
        [TestCase(Tree.saplingStage, false)]
        [TestCase(Tree.bushStage, false)]
        [TestCase(Tree.bushStage + 1, false)]
        [TestCase(Tree.treeStage, false)]
        public void DefaultsToVanilla(int treeStage, bool expectedPassability)
        {
            _tree.growthStage.Value = treeStage;
            _tree.isPassable().Should().Be(expectedPassability);
        }


        [TestCase(Tree.seedStage)]
        [TestCase(Tree.sproutStage)]
        [TestCase(Tree.saplingStage)]
        [TestCase(Tree.bushStage)]
        [TestCase(Tree.bushStage + 1)]
        [TestCase(Tree.treeStage)]
        public void MayOnlyPassLowerOrEqualStage(int maxPassableStage)
        {
            AggressiveAcorns.Config = new ConfigAdapter(new ModConfig {MaxPassibleGrowthStage = maxPassableStage});

            for (int stage = Tree.seedStage; stage <= Tree.treeStage; stage++)
            {
                _tree.growthStage.Value = stage;
                bool shouldPass = stage <= maxPassableStage;
                _tree.isPassable().Should().Be(shouldPass);
            }
        }

        [TestCase(-100, true)]
        [TestCase(-99, true)]
        [TestCase(-98, false)]
        [TestCase(0, false)]
        [TestCase(10, false)]
        public void MayPassDeadTrees(int health, bool expectedPassability)
        {
            AggressiveAcorns.Config = new ConfigAdapter(new ModConfig {MaxPassibleGrowthStage = -1});
            _tree.growthStage.Value = Tree.treeStage;
            _tree.health.Value = health;

            _tree.isPassable().Should().Be(expectedPassability);
        }
    }
}
