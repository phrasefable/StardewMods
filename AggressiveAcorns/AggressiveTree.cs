using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace AggressiveAcorns
{
    internal class AggressiveTree : Tree
    {
        private readonly IModConfig _config = AggressiveAcorns.Config;

        /// <summary>
        /// Flag to skip first update, used to prevent spread seeds from updating the night they are created.
        /// As spread seeds are not guaranteed to be hit in the update loop of the night they are planted, clearing this
        /// flag currently relies on the AggressiveTree -> Tree -> AggressiveTree conversion around serialization.
        /// </summary>
        private bool _skipUpdate;


        [UsedImplicitly]
        public AggressiveTree()
        {
        }


        public AggressiveTree([NotNull] Tree tree)
        {
            growthStage.Value = tree.growthStage.Value;
            treeType.Value = tree.treeType.Value;
            health.Value = tree.health.Value;
            flipped.Value = tree.flipped.Value;
            stump.Value = tree.stump.Value;
            tapped.Value = tree.tapped.Value;
            hasSeed.Value = tree.hasSeed.Value;
            fertilized.Value = tree.fertilized.Value;
        }


        private AggressiveTree(int treeType, int growthStage, bool skipFirstUpdate = false)
            : base(treeType, growthStage)
        {
            _skipUpdate = skipFirstUpdate;
        }


        [NotNull]
        public Tree ToTree()
        {
            var tree = new Tree();
            tree.growthStage.Value = growthStage.Value;
            tree.treeType.Value = treeType.Value;
            tree.health.Value = health.Value;
            tree.flipped.Value = flipped.Value;
            tree.stump.Value = stump.Value;
            tree.tapped.Value = tapped.Value;
            tree.hasSeed.Value = hasSeed.Value;
            tree.fertilized.Value = fertilized.Value;

            SyncFieldToTree<NetBool, bool>(tree, "destroy");

            return tree;
        }


        public override bool isPassable([CanBeNull] Character c = null)
        {
            return health.Value <= -99 || growthStage.Value <= _config.MaxPassibleGrowthStage;
        }


        public override void dayUpdate([NotNull] GameLocation environment, Vector2 tileLocation)
        {
            if (health.Value <= -100)
            {
                SetField<NetBool, bool>("destroy", true);
                _skipUpdate = true;
            }

            TreeUtils.ValidateTapped(this, environment, tileLocation);

            if (!_skipUpdate && TreeUtils.TreeCanGrow(this, environment, tileLocation))
            {
                TreeUtils.PopulateSeed(this);
                TreeUtils.TrySpread(this, environment, tileLocation);
                TreeUtils.TryIncreaseStage(this, environment, tileLocation);
                TreeUtils.ManageHibernation(this, environment, tileLocation);
                TreeUtils.TryRegrow(this, environment, tileLocation);
            }
            else
            {
                _skipUpdate = false;
            }

            // Revert to vanilla type early to prevent serialization issues in mods that serialize during the Saving event.
            // Relies on the fact that Terrain Feature iteration means that dayUpdate only won't be called again for the
            // same tileLocation.
            environment.terrainFeatures[tileLocation] = ToTree();
        }


        public override bool performToolAction(Tool t, int explosion, Vector2 tileLocation, GameLocation location)
        {
            bool prevent = _config.PreventScythe && t is MeleeWeapon;
            return !prevent && base.performToolAction(t, explosion, tileLocation, location);
        }


        // ===========================================================================================================


        private void SetField<TNetField, T>(string name, T value) where TNetField : NetField<T, TNetField>
        {
            AggressiveAcorns.ReflectionHelper.GetField<TNetField>(this, name).GetValue().Value = value;
        }


        private void SyncField<TNetField, T>(object origin, object target, string name)
            where TNetField : NetField<T, TNetField>
        {
            T value = AggressiveAcorns.ReflectionHelper.GetField<TNetField>(origin, name).GetValue().Value;
            AggressiveAcorns.ReflectionHelper.GetField<TNetField>(target, name).GetValue().Value = value;
        }


        private void SyncFieldToTree<TNetField, T>(Tree tree, string name) where TNetField : NetField<T, TNetField>
        {
            SyncField<TNetField, T>(this, tree, name);
        }
    }
}
