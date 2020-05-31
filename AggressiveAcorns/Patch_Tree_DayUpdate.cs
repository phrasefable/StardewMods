using System;
using System.Reflection;
using Common.Harmony;
using Common.Harmony.PatchValidation;
using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;
using static AggressiveAcorns.Utilities.Extensions.Tree;
using static AggressiveAcorns.Utilities.Extensions.GameLocation;
using static AggressiveAcorns.Utilities.TreeUtilities;

namespace AggressiveAcorns
{
    public static class Patch_Tree_DayUpdate
    {
        private static IMonitor _logger;
        private static IModConfig _config;
        private static IReflectionHelper _reflectionHelper;

        private static readonly MethodInfo PatchTarget = AccessTools.Method(
            typeof(Tree),
            nameof(Tree.dayUpdate),
            new[] {typeof(GameLocation), typeof(Vector2)}
        );

        private static readonly MethodInfo PatchSource = AccessTools.Method(
            typeof(Patch_Tree_DayUpdate),
            nameof(Patch_Tree_DayUpdate.TreeDayUpdate_Prefix)
        );


        internal static HarmonyPatchInfo Initialize(
            IMonitor monitor,
            IModConfig config,
            IReflectionHelper reflectionHelper)
        {
            _logger = monitor;
            _config = config;
            _reflectionHelper = reflectionHelper;

            return new HarmonyPatchInfo(PatchTarget, PatchSource, PrefixPatch.Instance, ExclusivePatch.Instance);
        }


        public static bool TreeDayUpdate_Prefix(
            Tree __instance,
            GameLocation environment,
            Vector2 tileLocation,
            NetBool ___destroy)
        {
            try
            {
                Update(__instance, environment, tileLocation, ___destroy);

                // Prevent other processing on the method.
                return false;
            }
            catch (Exception ex)
            {
                _logger.Log($"Harmony Patch failed in {PatchSource.Name}:\n{ex}", LogLevel.Error);
                // Allow original method (& other patches) to run.
                return true;
            }
        }


        // ============================================================================================================


        private static void Update(Tree tree, GameLocation location, Vector2 position, NetBool destroy)
        {
            // ===== Pre-processing =====

            if (tree.tapped.Value) ValidateTapped(tree, location, position);
            if (tree.health.Value <= -100) destroy.Value = true;

            // ===== Invalidation checks =====

            //TODO: check this; in vanilla only called when increasing stage
            if (!tree.MayGrowAt(location, position)) return;

            // ===== Processing =====

            if (tree.IsMushroomTree())
            {
                // Note short-circuit evaluation
                var changedHibernation =
                    ManageHibernation(tree, location, position) ||
                    TryRegrow(tree, location, position);

                // TODO: ensure non-hibernating immature trees have stump flag cleared.

                if (changedHibernation) return;

            }

            if (tree.stump.Value) return;

            if (tree.growthStage.Value >= Tree.treeStage)
            {
                TrySpread(tree, location, position);
            }
            else
            {
                TryIncreaseStage(tree, location, position);
            }

            if (tree.growthStage.Value >= Tree.treeStage)
            {
                PopulateSeed(tree); // TODO: Mushroom trees don't have seeds anyway.
            }
        }


        private static void ValidateTapped(Tree tree, GameLocation environment, Vector2 tileLocation)
        {
            var objectAtTile = environment.getObjectAtTile((int) tileLocation.X, (int) tileLocation.Y);
            /* TODO: magic number */
            if (objectAtTile == null || !objectAtTile.bigCraftable.Value || objectAtTile.ParentSheetIndex != 105)
            {
                tree.tapped.Value = false;
            }
        }


        private static void PopulateSeed(Tree tree)
        {
            /* Seed gain takes precedence over loss, hence loss is immaterial if it is just regained anyway */
            if (Game1.random.NextDouble() < _config.DailyChanceSeedGain)
            {
                tree.hasSeed.Value = true;
            }
            else if (Game1.random.NextDouble() < _config.DailyChanceSeedLoss)
            {
                tree.hasSeed.Value = false;
            }
        }


        private static void TrySpread(Tree tree, GameLocation location, Vector2 position)
        {
            // Invalidation
            if (!(location is Farm)) return;
            if (Game1.IsWinter && !_config.DoSpreadInWinter) return;
            if (tree.tapped.Value && !_config.DoTappedSpread) return;

            // Processing
            if (Game1.random.NextDouble() >= _config.DailyChanceSpread) return;

            foreach (var seedPos in position.GetSpreadLocations())
            {
                var tileX = (int) seedPos.X;
                var tileY = (int) seedPos.Y;
                if (_config.DoSeedsReplaceGrass &&
                    location.terrainFeatures.TryGetValue(seedPos, out var feature) &&
                    feature is Grass)
                {
                    PlaceOffspring(tree, location, seedPos);
                }
                else if (location.isTileLocationOpen(new Location(tileX * 64, tileY * 64))
                         && !location.isTileOccupied(seedPos)
                         && location.doesTileHaveProperty(tileX, tileY, "Water", "Back") == null
                         && location.isTileOnMap(seedPos))
                {
                    PlaceOffspring(tree, location, seedPos);
                }
            }
        }


        private static void PlaceOffspring(Tree tree, GameLocation location, Vector2 seedPosition)
        {
            tree.hasSeed.Value = false;

            var seed = new Tree(tree.treeType.Value, Tree.seedStage);
            location.terrainFeatures[seedPosition] = seed;
        }


        private static void TryIncreaseStage(Tree tree, GameLocation location, Vector2 position)
        {
            // TODO: only evaluate isShaded if necessary.
            var isShaded = location.HasShadeAt(position);

            // Invalidation
            // TODO reverse check.
            if (isShaded && tree.growthStage.Value >= _config.MaxShadedGrowthStage) return;

            /* Trees experiencing winter won't grow unless fertilized or set to ignore winter.
             * In addition to this, mushroom trees won't grow if they should be hibernating, even if fertilized. */
            if (location.ExperiencingWinter()
                && ((tree.IsMushroomTree() && _config.DoMushroomTreesHibernate)
                    || !(_config.DoGrowInWinter || tree.fertilized.Value)))
            {
                return;
            }

            // Processing
            if (_config.DoGrowInstantly)
            {
                tree.growthStage.Value = isShaded ? _config.MaxShadedGrowthStage : Tree.treeStage;
            }
            else if (Game1.random.NextDouble() < _config.DailyChanceGrowth || tree.fertilized.Value)
            {
                tree.growthStage.Value += 1;
            }
        }


        /* TODO: validate that all immature mushroom trees are only stumps if hibernating.
         * -> would that mean that RegrowStumpIfNotShaded only applies to mature trees, or should they regrow even if shaded?
         */
        private static bool ManageHibernation(Tree tree, GameLocation location, Vector2 position)
        {
            if (!_config.DoMushroomTreesHibernate) return false;
            if (!location.ExperiencesWinter()) return false;

            if (Game1.IsWinter)
            {
                tree.stump.Value = true;
                tree.health.Value = 5;
                return true;
            }

            if (Game1.IsSpring && Game1.dayOfMonth <= 1)
            {
                return RegrowStumpIfNotShaded(tree, location, position);
            }

            return false;
        }


        private static bool TryRegrow(Tree tree, GameLocation location, Vector2 position)
        {
            // Invalidation checks
            if (!_config.DoMushroomTreesRegrow) return false;
            if (!tree.stump.Value) return false;
            if (location.ExperiencingWinter() &&
                (_config.DoMushroomTreesHibernate || !_config.DoGrowInWinter)) return false;

            // Try Regrow
            if (_config.DoGrowInstantly || Game1.random.NextDouble() < _config.DailyChanceGrowth / 2)
            {
                return RegrowStumpIfNotShaded(tree, location, position);
            }

            return false;
        }


        private static bool RegrowStumpIfNotShaded(Tree tree, GameLocation location, Vector2 position)
        {
            if (location.HasShadeAt(position)) return false;

            tree.stump.Value = false;
            tree.health.Value = Tree.startingHealth;

            /* TODO: move this fix to the cause rather than this particular case where the symptoms become visible */
            _reflectionHelper.GetField<float>(tree, "shakeRotation").SetValue(0);

            return true;
        }
    }
}
