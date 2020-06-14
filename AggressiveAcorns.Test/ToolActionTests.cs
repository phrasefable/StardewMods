using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using StardewValley;
using StardewValley.Tools;

namespace Phrasefable.StardewMods.AggressiveAcorns.Test
{
    [TestFixture]
    public class ToolActionTests
    {
        // private AggressiveTree _tree;
        private ModConfig _config;
        private readonly GameLocation _dummyLocation = new DummyLocation();


        [SetUp]
        public void SetUp()
        {
            _config = new ModConfig();
            AggressiveAcorns.Config = _config;
            // _tree = new AggressiveTree();
        }


        [TestCase(false, true)] // Default (vanilla) - weapons effect trees
        [TestCase(true, false)] // Modded - blanket-prevent melee on trees
        public void MeleeWeaponDependsOnConfig(bool configValue, bool effectsTree)
        {
            this._config.PreventScythe = configValue;
            var shim = new ToolActionShim(effectsTree);
            var tree = new AggressiveTree(shim.Shim);

            bool result = tree.performToolAction(new MeleeWeapon(), 0, Vector2.Zero, this._dummyLocation);

            shim.Called.Should().Be(!configValue);
            result.Should().Be(effectsTree);
        }


        [Test]
        public void MeleeDelegatesCallAndUsesResultByDefault()
        {
            this.AssertDelegatesCallAndUsesResult<MeleeWeapon>();
        }


        [TestCase(true)]
        [TestCase(false)]
        public void OtherToolsDelegateCallAndUseResult(bool configValue)
        {
            this._config.PreventScythe = configValue;


            this.AssertDelegatesCallAndUsesResult<Axe>();
            this.AssertDelegatesCallAndUsesResult<Pickaxe>();
            this.AssertDelegatesCallAndUsesResult<Hoe>();
        }


        private void AssertDelegatesCallAndUsesResult<T>() where T : Tool, new()
        {
            foreach (bool fakeVanillaOutput in new[] {true, false})
            {
                // Arrange
                var shim = new ToolActionShim(fakeVanillaOutput);
                var tree = new AggressiveTree(shim.Shim);

                // Act
                bool result = tree.performToolAction(new T(), 0, Vector2.Zero, _dummyLocation);

                // Assert
                shim.Called.Should().BeTrue();
                result.Should().Be(fakeVanillaOutput);
            }
        }

        // ==================== Utility Classes ====================

        private class ToolActionShim
        {
            public bool Called { get; private set; }

            public readonly Func<Tool, int, Vector2, GameLocation, bool> Shim;

            public ToolActionShim(bool returnValue)
            {
                this.Shim = (t, i, v, l) =>
                {
                    Called = true;
                    return returnValue;
                };
            }
        }

        [SuppressMessage("ReSharper", "EmptyConstructor")]
        [SuppressMessage("ReSharper", "RedundantBaseConstructorCall")]
        private class DummyLocation : GameLocation
        {
            public DummyLocation() : base() { }
            protected override void initNetFields() { }
        }
    }
}
