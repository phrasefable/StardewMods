using System;
using System.Collections.Generic;
using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
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
            NetBool ___destroy)
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
                    ManageHibernation(__instance, environment, tileLocation);
                    TrySpread(__instance, environment, tileLocation);
                    PopulateSeed(__instance);
                }
            }

            return false;
        }

        private static void TryIncreaseStage(Tree tree, GameLocation environment, Vector2 tile)
        {
            if (tree.growthStage.Value >= Tree.treeStage ||
                (tree.growthStage.Value >= _config.iMaxShadedGrowthStage && IsShaded(environment, tile)) ||
                (tree.treeType.Value == Tree.mushroomTree && _config.bDoMushroomTreesHibernate) ||
                !IsGrowthSeason(environment)) return;

            if (_config.bDoGrowInstantly)
            {
                tree.growthStage.Value = Tree.treeStage;
            }
            else if (Game1.random.NextDouble() < _config.fDailyGrowthChance)
            {
                tree.growthStage.Value += 1;
            }
        }

        private static void ManageHibernation(Tree tree, GameLocation environment, Vector2 tile)
        {
            // rn, only mushroom trees will hibernate
            if (tree.treeType.Value != Tree.mushroomTree) return;

            var isWinter = Game1.currentSeason.Equals("winter");
            if (_config.bDoMushroomTreesHibernate)
            {
                if (isWinter)
                {
                    tree.stump.Value = true;
                }
                else if (Game1.currentSeason.Equals("spring") && Game1.dayOfMonth <= 1)
                {
                    tree.stump.Value = false;
                    tree.health.Value = Tree.startingHealth;
                }
            }

            if (_config.bDoMushroomTreesRegrow && !isWinter && tree.stump.Value &&
                (_config.bDoGrowInstantly || Game1.random.NextDouble() < _config.fDailyGrowthChance / 2))
            {
                tree.stump.Value = false;
                tree.health.Value = Tree.startingHealth;
            }
        }

        private static void TrySpread(Tree tree, GameLocation environment, Vector2 tile)
        {
            if (!(environment is Farm) ||
                tree.growthStage.Value >= Tree.treeStage ||
                (Game1.currentSeason.Equals("winter") && !_config.bDoSpreadInWinter) ||
                (tree.tapped.Value && !_config.bDoTappedSpread) ||
                tree.stump.Value) return;

            foreach (var seedPos in GetSpreadLocation(tile))
            {
                var tileX = (int) seedPos.X;
                var tileY = (int) seedPos.Y;
                if (_config.bSeedsReplaceGrass && environment.terrainFeatures.TryGetValue(seedPos, out var feature) &&
                    feature is Grass)
                {
                    environment.terrainFeatures[seedPos] = new Tree(tree.treeType.Value, 0);
                    tree.hasSeed.Value = false;
                }
                else if (environment.isTileLocationOpen(new Location(tileX * 64, tileY * 64))
                         && !environment.isTileOccupied(seedPos, "")
                         && environment.doesTileHaveProperty(tileX, tileY, "Water", "Back") == null
                         && environment.isTileOnMap(seedPos))
                {
                    environment.terrainFeatures.Add(seedPos, new Tree(tree.treeType.Value, 0));
                    tree.hasSeed.Value = false;
                }
            }
        }

        private static IEnumerable<Vector2> GetSpreadLocation(Vector2 tile)
        {
//            return Utility.getSurroundingTileLocationsArray(tile);
            // pick random tile within +-3 x/y.
            if (Game1.random.NextDouble() < _config.fDailySpreadChance)
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

            if (!_config.bDoSeedsPersist)
            {
                tree.hasSeed.Value = false;
            }

            if (Game1.random.NextDouble() < _config.fDailySeedChance)
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

        private static bool IsGrowthSeason(GameLocation environment)
        {
            return
                !Game1.currentSeason.Equals("winter")
                || _config.bDoGrowInWinter
                // Covers greenhouse
                || !environment.IsOutdoors
                // If palm trees elsewhere (mod?), they will follow normal tree rules.
                // If normal trees in desert (hoe), they will grow through winter.
                || environment is Desert;
        }

        private static bool IsShaded(GameLocation environment, Vector2 tile)
        {
            var surrounds = Utility.getSurroundingTileLocationsArray(tile);
            foreach (var adjacentTile in surrounds)
            {
                if (environment.terrainFeatures.ContainsKey(adjacentTile)
                    && environment.terrainFeatures[adjacentTile] is Tree adjTree
                    && adjTree.growthStage.Value > _config.iMaxShadedGrowthStage)
                {
                    return true;
                }
            }

            return false;
        }
    }
}