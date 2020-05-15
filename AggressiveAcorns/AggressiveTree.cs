using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using xTile.Dimensions;

namespace Phrasefable.StardewMods.AggressiveAcorns
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
        public AggressiveTree() { }


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


        private static void SyncField<TNetField, T>(object origin, object target, string name)
            where TNetField : NetField<T, TNetField>
        {
            T value = AggressiveAcorns.ReflectionHelper.GetField<TNetField>(origin, name).GetValue().Value;
            AggressiveAcorns.ReflectionHelper.GetField<TNetField>(target, name).GetValue().Value = value;
        }


        private void SyncFieldToTree<TNetField, T>(Tree tree, string name) where TNetField : NetField<T, TNetField>
        {
            AggressiveTree.SyncField<TNetField, T>(this, tree, name);
        }
    }


    internal static class TreeUtils
    {
        public static void ValidateTapped(Tree tree, GameLocation environment, Vector2 tileLocation)
        {
            if (!tree.tapped.Value) return;

            Object objectAtTile = environment.getObjectAtTile((int) tileLocation.X, (int) tileLocation.Y);
            if (objectAtTile == null || !objectAtTile.bigCraftable.Value || objectAtTile.ParentSheetIndex != 105)
            {
                tree.tapped.Value = false;
            }
        }

        public static void TryIncreaseStage(Tree tree, GameLocation location, Vector2 position)
        {
            if (IsFullyGrown(tree) || (tree.growthStage.Value >= AggressiveAcorns.Config.MaxShadedGrowthStage &&
                                       IsShaded(location, position)))
            {
                return;
            }

            // Trees experiencing winter won't grow unless fertilized or set to ignore winter.
            // In addition to this, mushroom trees won't grow if they should be hibernating, even if fertilized.
            if (ExperiencingWinter(location)
                && ((IsMushroomTree(tree) && AggressiveAcorns.Config.DoMushroomTreesHibernate)
                    || !(AggressiveAcorns.Config.DoGrowInWinter || tree.fertilized.Value)))
            {
                return;
            }

            if (AggressiveAcorns.Config.DoGrowInstantly)
            {
                tree.growthStage.Value = IsShaded(location, position)
                    ? AggressiveAcorns.Config.MaxShadedGrowthStage
                    : Tree.treeStage;
            }
            else if (Game1.random.NextDouble() < AggressiveAcorns.Config.DailyGrowthChance || tree.fertilized.Value)
            {
                tree.growthStage.Value += 1;
            }
        }


        public static void ManageHibernation(Tree tree, GameLocation location, Vector2 position)
        {
            if (!IsMushroomTree(tree)
                || !AggressiveAcorns.Config.DoMushroomTreesHibernate
                || !ExperiencesWinter(location))
            {
                return;
            }

            if (Game1.IsWinter)
            {
                tree.stump.Value = true;
                tree.health.Value = 5;
            }
            else if (Game1.IsSpring && Game1.dayOfMonth <= 1)
            {
                RegrowStumpIfNotShaded(tree, location, position);
            }
        }


        public static void TryRegrow(Tree tree, GameLocation location, Vector2 position)
        {
            if (IsMushroomTree(tree) &&
                AggressiveAcorns.Config.DoMushroomTreesRegrow &&
                tree.stump.Value &&
                (!ExperiencingWinter(location) || (!AggressiveAcorns.Config.DoMushroomTreesHibernate &&
                                                   AggressiveAcorns.Config.DoGrowInWinter)) &&
                (AggressiveAcorns.Config.DoGrowInstantly ||
                 Game1.random.NextDouble() < AggressiveAcorns.Config.DailyGrowthChance / 2))
            {
                TreeUtils.RegrowStumpIfNotShaded(tree, location, position);
            }
        }


        public static void RegrowStumpIfNotShaded(Tree tree, GameLocation location, Vector2 position)
        {
            if (TreeUtils.IsShaded(location, position)) return;

            tree.stump.Value = false;
            tree.health.Value = Tree.startingHealth;

            /*  Not currently needed as AggressiveTree is converted to Tree and back around save to allow
             *  serialization (ie. new objects created so rotation is reset).
             *  If this changes (ie. Aggressive Tree cached over save or otherwise reused), must re-enable below code.
             */
            // AggressiveAcorns.ReflectionHelper.GetField<float>(tree, "shakeRotation").SetValue(0);
        }


        public static void TrySpread(Tree tree, GameLocation location, Vector2 position)
        {
            if (!(location is Farm) ||
                !TreeUtils.IsFullyGrown(tree) ||
                (Game1.IsWinter && !AggressiveAcorns.Config.DoSpreadInWinter) ||
                (tree.tapped.Value && !AggressiveAcorns.Config.DoTappedSpread) ||
                tree.stump.Value)
            {
                return;
            }

            foreach (Vector2 seedPos in TreeUtils.GetSpreadLocations(position))
            {
                var tileX = (int) seedPos.X;
                var tileY = (int) seedPos.Y;
                if (AggressiveAcorns.Config.SeedsReplaceGrass &&
                    location.terrainFeatures.TryGetValue(seedPos, out TerrainFeature feature) &&
                    feature is Grass)
                {
                    TreeUtils.PlaceOffspring(tree, location, seedPos);
                }
                else if (location.isTileLocationOpen(new Location(tileX * 64, tileY * 64))
                         && !location.isTileOccupied(seedPos)
                         && location.doesTileHaveProperty(tileX, tileY, "Water", "Back") == null
                         && location.isTileOnMap(seedPos))
                {
                    TreeUtils.PlaceOffspring(tree, location, seedPos);
                }
            }
        }


        public static void PlaceOffspring(Tree tree, GameLocation location, Vector2 seedPosition)
        {
            tree.hasSeed.Value = false;

            var seed = new Tree(tree.treeType.Value, 0);
            location.terrainFeatures[seedPosition] = seed;
        }


        public static IEnumerable<Vector2> GetSpreadLocations(Vector2 position)
        {
            // pick random tile within +-3 x/y.
            if (Game1.random.NextDouble() < AggressiveAcorns.Config.DailySpreadChance)
            {
                int tileX = Game1.random.Next(-3, 4) + (int) position.X;
                int tileY = Game1.random.Next(-3, 4) + (int) position.Y;
                var seedPos = new Vector2(tileX, tileY);
                yield return seedPos;
            }
        }


        public static void PopulateSeed(Tree tree)
        {
            if (!TreeUtils.IsFullyGrown(tree) || tree.stump.Value) return;

            if (!AggressiveAcorns.Config.DoSeedsPersist)
            {
                tree.hasSeed.Value = false;
            }

            if (Game1.random.NextDouble() < AggressiveAcorns.Config.DailySeedChance)
            {
                tree.hasSeed.Value = true;
            }
        }


        public static bool TreeCanGrow(Tree tree, GameLocation location, Vector2 position)
        {
            string prop = location.doesTileHaveProperty((int) position.X, (int) position.Y, "NoSpawn", "Back");
            bool tileCanSpawnTree = prop == null || !(prop.Equals("All") || prop.Equals("Tree") || prop.Equals("True"));
            bool isBlockedSeed = tree.growthStage.Value == 0 && location.objects.ContainsKey(position);
            return tileCanSpawnTree && !isBlockedSeed;
        }


        public static bool ExperiencingWinter(GameLocation location)
        {
            return Game1.IsWinter && TreeUtils.ExperiencesWinter(location);
        }


        public static bool ExperiencesWinter(GameLocation location)
        {
            return location.IsOutdoors && !(location is Desert);
        }


        public static bool IsShaded(GameLocation location, Vector2 position)
        {
            foreach (Vector2 adjacentTile in Utility.getSurroundingTileLocationsArray(position))
            {
                if (location.terrainFeatures.TryGetValue(adjacentTile, out TerrainFeature feature)
                    && feature is Tree adjTree
                    && TreeUtils.IsFullyGrown(adjTree)
                    && !adjTree.stump.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsFullyGrown(Tree tree)
        {
            return tree.growthStage.Value >= Tree.treeStage;
        }

        public static bool IsMushroomTree(Tree tree)
        {
            return tree.treeType.Value == Tree.mushroomTree;
        }
    }
}
