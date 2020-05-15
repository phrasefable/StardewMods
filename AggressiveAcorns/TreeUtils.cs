using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;

namespace AggressiveAcorns
{
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
                RegrowStumpIfNotShaded(tree, location, position);
            }
        }


        public static void RegrowStumpIfNotShaded(Tree tree, GameLocation location, Vector2 position)
        {
            if (IsShaded(location, position)) return;

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
                !IsFullyGrown(tree) ||
                (Game1.IsWinter && !AggressiveAcorns.Config.DoSpreadInWinter) ||
                (tree.tapped.Value && !AggressiveAcorns.Config.DoTappedSpread) ||
                tree.stump.Value)
            {
                return;
            }

            foreach (Vector2 seedPos in GetSpreadLocations(position))
            {
                var tileX = (int) seedPos.X;
                var tileY = (int) seedPos.Y;
                if (AggressiveAcorns.Config.SeedsReplaceGrass &&
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
            if (!IsFullyGrown(tree) || tree.stump.Value) return;

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
            return Game1.IsWinter && ExperiencesWinter(location);
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
                    && IsFullyGrown(adjTree)
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