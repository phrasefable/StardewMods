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


        private static void Update(Tree tree, GameLocation environment, Vector2 tileLocation, NetBool destroy)
        {
            // ===== Pre-processing =====

            ValidateTapped(tree, environment, tileLocation);

            // ===== Invalidation checks =====

            if (tree.health.Value <= -100)
            {
                destroy.Value = true;
                return;
            }

            if (tree.MayGrowAt(environment, tileLocation)) return;

            // ===== Processing =====

            if (tree.IsMushroomTree())
            {
                ManageHibernation(tree, environment, tileLocation);
                TryRegrow(tree, environment, tileLocation);
                /* TODO: prevent further processing if hibernates/regrows*/
            }

            if (tree.IsFullyGrown())
            {
                if (!tree.stump.Value)
                {
                    PopulateSeed(tree);
                    TrySpread(tree, environment, tileLocation);
                }
            }
            else
            {
                TryIncreaseStage(tree, environment, tileLocation);
            }
        }


        private static void ValidateTapped(Tree tree, GameLocation environment, Vector2 tileLocation)
        {
            if (!tree.tapped.Value) return;

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
            var isShaded = location.HasShadeAt(position);

            // Invalidation
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


        private static void ManageHibernation(Tree tree, GameLocation location, Vector2 position)
        {
            if (!_config.DoMushroomTreesHibernate) return;
            if (!location.ExperiencesWinter()) return;

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


        private static void TryRegrow(Tree tree, GameLocation location, Vector2 position)
        {
            // Invalidation checks
            if (!_config.DoMushroomTreesRegrow) return;
            if (!tree.stump.Value) return;
            if (location.ExperiencingWinter() &&
                (_config.DoMushroomTreesHibernate || !_config.DoGrowInWinter)) return;

            // Try Regrow
            if (_config.DoGrowInstantly || Game1.random.NextDouble() < _config.DailyChanceGrowth / 2)
            {
                RegrowStumpIfNotShaded(tree, location, position);
            }
        }


        private static void RegrowStumpIfNotShaded(Tree tree, GameLocation location, Vector2 position)
        {
            if (location.HasShadeAt(position)) return;

            tree.stump.Value = false;
            tree.health.Value = Tree.startingHealth;

            _reflectionHelper.GetField<float>(tree, "shakeRotation").SetValue(0);
        }
    }
}
