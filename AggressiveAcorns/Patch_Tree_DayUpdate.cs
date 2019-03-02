using System;
using System.Collections.Generic;
using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using PhraseLib;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;

namespace AggressiveAcorns
{
    public class Patch_Tree_DayUpdate : PrefixPatch
    {
        private static IModConfig _config;

        protected override Type TargetType => typeof(Tree);

        protected override string TargetName => nameof(Tree.dayUpdate);

        protected override Type[] TargetParameters => new[]
        {
            typeof(GameLocation),
            typeof(Vector2)
        };

        public Patch_Tree_DayUpdate(IModConfig config)
        {
            _config = config;
        }

        protected override string PatchMethod => nameof(TreeDayUpdate);

        public override bool IsValid(HarmonyInstance harmony, out string errors)
        {
            return IsExclusivePatch(harmony, out errors);
        }

        private static bool TreeDayUpdate(Tree __instance, GameLocation environment, Vector2 tileLocation,
            NetBool ___destroy, ref float ___shakeRotation)
        {
            if (__instance.health.Value <= -100)
            {
                ___destroy.Value = true;
            }
            else
            {
                var treeCanGrow = TreeCanGrow(__instance, environment, tileLocation);
                if (treeCanGrow)
                {
                    TryIncreaseStage(__instance, environment, tileLocation);
                    ManageHibernation(__instance, environment, tileLocation, ref ___shakeRotation);
                    TryRegrow(__instance, environment, tileLocation, ref ___shakeRotation);
                    TrySpread(__instance, environment, tileLocation);
                    PopulateSeed(__instance);
                }
            }

            return false;
        }

        private static void TryIncreaseStage(Tree tree, GameLocation environment, Vector2 tile)
        {
            if (tree.growthStage.Value >= Tree.treeStage ||
                (tree.growthStage.Value >= _config.MaxShadedGrowthStage && IsShaded(environment, tile)) ||
                (tree.treeType.Value == Tree.mushroomTree && _config.DoMushroomTreesHibernate) ||
                (!_config.DoGrowInWinter && CountsAsWinter(environment))) return;

            if (_config.DoGrowInstantly)
            {
                tree.growthStage.Value = Tree.treeStage;
            }
            else if (Game1.random.NextDouble() < _config.DailyGrowthChance)
            {
                tree.growthStage.Value += 1;
            }
        }

        private static void ManageHibernation(Tree tree, GameLocation environment, Vector2 tile, ref float rotation)
        {
            // only mushroom trees will hibernate and iff it gets cold enough
            if (tree.treeType.Value != Tree.mushroomTree || !ExperiencesWinter(environment)) return;

            if (_config.DoMushroomTreesHibernate)
            {
                if (Game1.currentSeason.Equals("winter"))
                {
                    tree.stump.Value = true;
                    tree.health.Value = 5;
                }
                else if (Game1.currentSeason.Equals("spring") && Game1.dayOfMonth <= 1)
                {
                    RegrowStumpIfNotShaded(tree, environment, tile, ref rotation);
                }
            }
        }

        private static void TryRegrow(Tree tree, GameLocation environment, Vector2 tile, ref float rotation)
        {
            if (tree.treeType.Value == Tree.mushroomTree &&
                _config.DoMushroomTreesRegrow &&
                tree.stump.Value &&
                (!CountsAsWinter(environment) || (!_config.DoMushroomTreesHibernate && _config.DoGrowInWinter)) &&
                (_config.DoGrowInstantly || Game1.random.NextDouble() < _config.DailyGrowthChance / 2))
            {
                RegrowStumpIfNotShaded(tree, environment, tile, ref rotation);
            }
        }

        private static void RegrowStumpIfNotShaded(Tree tree, GameLocation environment, Vector2 tile,
            ref float rotation)
        {
            if (IsShaded(environment, tile)) return;

            tree.stump.Value = false;
            tree.health.Value = Tree.startingHealth;
            rotation = 0;
        }

        private static void TrySpread(Tree tree, GameLocation environment, Vector2 tile)
        {
            if (!(environment is Farm) ||
                tree.growthStage.Value < Tree.treeStage ||
                (Game1.currentSeason.Equals("winter") && !_config.DoSpreadInWinter) ||
                (tree.tapped.Value && !_config.DoTappedSpread) ||
                tree.stump.Value) return;

            foreach (var seedPos in GetSpreadLocations(tile))
            {
                var tileX = (int) seedPos.X;
                var tileY = (int) seedPos.Y;
                if (_config.SeedsReplaceGrass && environment.terrainFeatures.TryGetValue(seedPos, out var feature) &&
                    feature is Grass)
                {
                    environment.terrainFeatures[seedPos] = new Tree(tree.treeType.Value, 0);
                    tree.hasSeed.Value = false;
                }
                else if (environment.isTileLocationOpen(new Location(tileX * 64, tileY * 64))
                         && !environment.isTileOccupied(seedPos)
                         && environment.doesTileHaveProperty(tileX, tileY, "Water", "Back") == null
                         && environment.isTileOnMap(seedPos))
                {
                    environment.terrainFeatures.Add(seedPos, new Tree(tree.treeType.Value, 0));
                    tree.hasSeed.Value = false;
                }
            }
        }

        private static IEnumerable<Vector2> GetSpreadLocations(Vector2 tile)
        {
            // pick random tile within +-3 x/y.
            if (Game1.random.NextDouble() < _config.DailySpreadChance)
            {
                var tileX = Game1.random.Next(-3, 4) + (int) tile.X;
                var tileY = Game1.random.Next(-3, 4) + (int) tile.Y;
                var seedPos = new Vector2(tileX, tileY);
                yield return seedPos;
            }
        }

        private static void PopulateSeed(Tree tree)
        {
            if (tree.growthStage.Value < Tree.treeStage || tree.stump.Value) return;

            if (!_config.DoSeedsPersist)
            {
                tree.hasSeed.Value = false;
            }

            if (Game1.random.NextDouble() < _config.DailySeedChance)
            {
                tree.hasSeed.Value = true;
            }
        }

        private static bool TreeCanGrow(Tree tree, GameLocation environment, Vector2 location)
        {
            var canSpawn = TileCanSpawnTree(environment, location);
            var isBlockedSeed = tree.growthStage.Value == 0 && environment.objects.ContainsKey(location);
            return canSpawn && !isBlockedSeed;
        }

        private static bool TileCanSpawnTree(GameLocation environment, Vector2 location)
        {
            var str = environment.doesTileHaveProperty((int) location.X, (int) location.Y, "NoSpawn", "Back");
            return str == null || !(str.Equals("All") || str.Equals("Tree") || str.Equals("True"));
        }

        private static bool CountsAsWinter(GameLocation environment)
        {
            return Game1.currentSeason.Equals("winter") && ExperiencesWinter(environment);
        }

        private static bool ExperiencesWinter(GameLocation environment)
        {
            return environment.IsOutdoors && !(environment is Desert);
        }

        private static bool IsShaded(GameLocation environment, Vector2 tile)
        {
            foreach (var adjacentTile in Utility.getSurroundingTileLocationsArray(tile))
            {
                if (environment.terrainFeatures.TryGetValue(adjacentTile, out var feature)
                    && feature is Tree adjTree
                    && adjTree.growthStage.Value >= Tree.treeStage
                    && !adjTree.stump.Value)
                {
                    return true;
                }
            }

            return false;
        }
    }
}