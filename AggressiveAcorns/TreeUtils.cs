using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;

namespace Phrasefable.StardewMods.AggressiveAcorns
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
            if (TreeUtils.IsFullyGrown(tree) ||
                (tree.growthStage.Value >= AggressiveAcorns.Config.MaxShadedGrowthStage &&
                 TreeUtils.IsShaded(location, position)))
            {
                return;
            }

            // Trees experiencing winter won't grow unless fertilized or set to ignore winter.
            // In addition to this, mushroom trees won't grow if they should be hibernating, even if fertilized.
            if (TreeUtils.ExperiencingWinter(location)
                && ((TreeUtils.IsMushroomTree(tree) && AggressiveAcorns.Config.DoMushroomTreesHibernate)
                    || !(AggressiveAcorns.Config.DoGrowInWinter || tree.fertilized.Value)))
            {
                return;
            }

            if (AggressiveAcorns.Config.DoGrowInstantly)
            {
                tree.growthStage.Value = TreeUtils.IsShaded(location, position)
                    ? AggressiveAcorns.Config.MaxShadedGrowthStage
                    : Tree.treeStage;
            }
            else if (AggressiveAcorns.Config.RollForGrowth || tree.fertilized.Value)
            {
                tree.growthStage.Value += 1;
            }
        }


        public static void ManageHibernation(Tree tree, GameLocation location, Vector2 position)
        {
            if (!TreeUtils.IsMushroomTree(tree)
                || !AggressiveAcorns.Config.DoMushroomTreesHibernate
                || !TreeUtils.ExperiencesWinter(location))
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
                TreeUtils.RegrowStumpIfNotShaded(tree, location, position);
            }
        }


        public static void TryRegrow(Tree tree, GameLocation location, Vector2 position)
        {
            if (TreeUtils.IsMushroomTree(tree) &&
                AggressiveAcorns.Config.DoMushroomTreesRegrow &&
                tree.stump.Value &&
                (!TreeUtils.ExperiencingWinter(location) || (!AggressiveAcorns.Config.DoMushroomTreesHibernate &&
                                                             AggressiveAcorns.Config.DoGrowInWinter)) &&
                (AggressiveAcorns.Config.DoGrowInstantly || AggressiveAcorns.Config.RollForMushroomRegrowth))
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

            foreach (Vector2 offset in AggressiveAcorns.Config.SpreadSeedOffsets)
            {
                Vector2 seedPos = position + offset;
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


        public static IEnumerable<Vector2> GetSpreadOffsets()
        {
            if (!AggressiveAcorns.Config.RollForSpread) yield break;

            static int GetOffset() => Game1.random.Next(-3, 4);
            yield return new Vector2(GetOffset(), GetOffset());
        }


        public static void PopulateSeed(Tree tree)
        {
            if (!TreeUtils.IsFullyGrown(tree) || tree.stump.Value) return;

            if (!AggressiveAcorns.Config.DoSeedsPersist)
            {
                tree.hasSeed.Value = false;
            }

            if (AggressiveAcorns.Config.RollForSeed)
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
