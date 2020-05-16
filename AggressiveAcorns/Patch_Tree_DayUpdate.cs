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
using Object = StardewValley.Object;

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
            nameof(Patch_Tree_DayUpdate.TreeDayUpdate)
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


        public static bool TreeDayUpdate(
            Tree __instance,
            GameLocation environment,
            Vector2 tileLocation,
            NetBool ___destroy)
        {
            try
            {
                bool isDestroyed = DestroyIfDead(__instance, ___destroy);

                ValidateTapped(__instance, environment, tileLocation);

                if (!isDestroyed && Queries.TreeCanGrow(__instance, environment, tileLocation))
                {
                    PopulateSeed(__instance);
                    TrySpread(__instance, environment, tileLocation);
                    TryIncreaseStage(__instance, environment, tileLocation);
                    ManageHibernation(__instance, environment, tileLocation);
                    TryRegrow(__instance, environment, tileLocation);
                }

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


        private static bool DestroyIfDead(Tree tree, NetBool destroy)
        {
            if (tree.health.Value > -100) return false;

            destroy.Value = true;
            return true;
        }


        private static void ValidateTapped(Tree tree, GameLocation environment, Vector2 tileLocation)
        {
            if (!tree.tapped.Value) return;

            Object objectAtTile = environment.getObjectAtTile((int) tileLocation.X, (int) tileLocation.Y);
            if (objectAtTile == null || !objectAtTile.bigCraftable.Value || objectAtTile.ParentSheetIndex != 105)
            {
                tree.tapped.Value = false;
            }
        }


        private static void PopulateSeed(Tree tree)
        {
            if (!Queries.IsFullyGrown(tree) || tree.stump.Value) return;

            if (!_config.DoSeedsPersist)
            {
                tree.hasSeed.Value = false;
            }

            if (Game1.random.NextDouble() < _config.DailySeedChance)
            {
                tree.hasSeed.Value = true;
            }
        }


        private static void TrySpread(Tree tree, GameLocation location, Vector2 position)
        {
            if (!(location is Farm) ||
                !Queries.IsFullyGrown(tree) ||
                (Game1.IsWinter && !_config.DoSpreadInWinter) ||
                (tree.tapped.Value && !_config.DoTappedSpread) ||
                tree.stump.Value)
            {
                return;
            }

            if (Game1.random.NextDouble() >= _config.DailySpreadChance) return;

            foreach (Vector2 seedPos in Queries.GetSpreadLocations(position))
            {
                var tileX = (int) seedPos.X;
                var tileY = (int) seedPos.Y;
                if (_config.SeedsReplaceGrass &&
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

            var seed = new Tree(tree.treeType.Value, 0);
            location.terrainFeatures[seedPosition] = seed;
        }


        private static void TryIncreaseStage(Tree tree, GameLocation location, Vector2 position)
        {
            if (Queries.IsFullyGrown(tree) ||
                (tree.growthStage.Value >= _config.MaxShadedGrowthStage &&
                 Queries.IsShaded(location, position)))
            {
                return;
            }

            // Trees experiencing winter won't grow unless fertilized or set to ignore winter.
            // In addition to this, mushroom trees won't grow if they should be hibernating, even if fertilized.
            if (Queries.ExperiencingWinter(location)
                && ((Queries.IsMushroomTree(tree) && _config.DoMushroomTreesHibernate)
                    || !(_config.DoGrowInWinter || tree.fertilized.Value)))
            {
                return;
            }

            if (_config.DoGrowInstantly)
            {
                tree.growthStage.Value = Queries.IsShaded(location, position)
                    ? _config.MaxShadedGrowthStage
                    : Tree.treeStage;
            }
            else if (Game1.random.NextDouble() < _config.DailyGrowthChance || tree.fertilized.Value)
            {
                tree.growthStage.Value += 1;
            }
        }


        private static void ManageHibernation(Tree tree, GameLocation location, Vector2 position)
        {
            if (!Queries.IsMushroomTree(tree)
                || !_config.DoMushroomTreesHibernate
                || !Queries.ExperiencesWinter(location))
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


        private static void TryRegrow(Tree tree, GameLocation location, Vector2 position)
        {
            if (Queries.IsMushroomTree(tree) &&
                _config.DoMushroomTreesRegrow &&
                tree.stump.Value &&
                (!Queries.ExperiencingWinter(location) || (!_config.DoMushroomTreesHibernate &&
                                                           _config.DoGrowInWinter)) &&
                (_config.DoGrowInstantly ||
                 Game1.random.NextDouble() < _config.DailyGrowthChance / 2))
            {
                RegrowStumpIfNotShaded(tree, location, position);
            }
        }


        private static void RegrowStumpIfNotShaded(Tree tree, GameLocation location, Vector2 position)
        {
            if (Queries.IsShaded(location, position)) return;

            tree.stump.Value = false;
            tree.health.Value = Tree.startingHealth;

            _reflectionHelper.GetField<float>(tree, "shakeRotation").SetValue(0);
        }
    }
}
