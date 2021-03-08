using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;

namespace Phrasefable.StardewMods.AggressiveAcorns
{
    internal static class TreeUtils
    {
        public static bool DestroyIfDead(this Tree tree, NetBool destroy)
        {
            if (tree.health.Value > -100) return false;

            destroy.Value = true;
            return true;
        }


        public static void ValidateTapped(this Tree tree, GameLocation environment, Vector2 tileLocation)
        {
            if (!tree.tapped.Value) return;

            Object objectAtTile = environment.getObjectAtTile((int) tileLocation.X, (int) tileLocation.Y);
            if (objectAtTile == null || !objectAtTile.bigCraftable.Value || objectAtTile.ParentSheetIndex != 105)
            {
                tree.tapped.Value = false;
            }
        }


        public static void TryIncreaseStage(this Tree tree, GameLocation location, Vector2 position)
        {
            if (tree.IsFullyGrown() ||
                (tree.growthStage.Value >= AggressiveAcorns.Config.MaxShadedGrowthStage &&
                 location.IsShadedAt(position)))
            {
                return;
            }

            // Trees experiencing winter won't grow unless fertilized or set to ignore winter.
            // In addition to this, mushroom trees won't grow if they should be hibernating, even if fertilized.
            if (location.ExperiencingWinter()
                && ((tree.IsMushroomTree() && AggressiveAcorns.Config.DoMushroomTreesHibernate)
                    || !(AggressiveAcorns.Config.DoGrowInWinter || tree.fertilized.Value)))
            {
                return;
            }

            if (AggressiveAcorns.Config.DoGrowInstantly)
            {
                tree.growthStage.Value = location.IsShadedAt(position)
                    ? AggressiveAcorns.Config.MaxShadedGrowthStage
                    : Tree.treeStage;
            }
            else if (AggressiveAcorns.Config.RollForGrowth || tree.fertilized.Value)
            {
                tree.growthStage.Value += 1;
            }
        }


        public static void ManageHibernation(this Tree tree, GameLocation location, Vector2 position)
        {
            if (!tree.IsMushroomTree()
                || !AggressiveAcorns.Config.DoMushroomTreesHibernate
                || !location.ExperiencesWinter())
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
                tree.RegrowStumpIfNotShaded(location, position);
            }
        }


        public static void TryRegrow(this Tree tree, GameLocation location, Vector2 position)
        {
            if (tree.IsMushroomTree() &&
                AggressiveAcorns.Config.DoMushroomTreesRegrow &&
                tree.stump.Value &&
                (!location.ExperiencingWinter() || (!AggressiveAcorns.Config.DoMushroomTreesHibernate &&
                                                    AggressiveAcorns.Config.DoGrowInWinter)) &&
                (AggressiveAcorns.Config.DoGrowInstantly || AggressiveAcorns.Config.RollForMushroomRegrowth))
            {
                tree.RegrowStumpIfNotShaded(location, position);
            }
        }


        public static void RegrowStumpIfNotShaded(this Tree tree, GameLocation location, Vector2 position)
        {
            if (location.IsShadedAt(position)) return;

            tree.stump.Value = false;
            tree.health.Value = Tree.startingHealth;

            /*  Not currently needed as AggressiveTree is converted to Tree and back around save to allow
             *  serialization (ie. new objects created so rotation is reset).
             *  If this changes (ie. Aggressive Tree cached over save or otherwise reused), must re-enable below code.
             */
            // AggressiveAcorns.ReflectionHelper.GetField<float>(tree, "shakeRotation").SetValue(0);
        }


        public static void TrySpread(this Tree tree, GameLocation location, Vector2 position)
        {
            if (!(location is Farm) ||
                !tree.IsFullyGrown() ||
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
                    tree.PlaceOffspring(location, seedPos);
                }
                else if (location.isTileLocationOpen(new Location(tileX * 64, tileY * 64))
                         && !location.isTileOccupied(seedPos)
                         && location.doesTileHaveProperty(tileX, tileY, "Water", "Back") == null
                         && location.isTileOnMap(seedPos))
                {
                    tree.PlaceOffspring(location, seedPos);
                }
            }
        }


        public static void PlaceOffspring(this Tree tree, GameLocation location, Vector2 seedPosition)
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


        public static void PopulateSeed(this Tree tree)
        {
            if (!tree.IsFullyGrown() || tree.stump.Value) return;

            if (!AggressiveAcorns.Config.DoSeedsPersist)
            {
                tree.hasSeed.Value = false;
            }

            if (AggressiveAcorns.Config.RollForSeed)
            {
                tree.hasSeed.Value = true;
            }
        }


        public static bool TreeCanGrowAt(this GameLocation location, Tree tree, Vector2 position)
        {
            string prop = location.doesTileHaveProperty((int) position.X, (int) position.Y, "NoSpawn", "Back");
            bool tileCanSpawnTree = prop == null || !(prop.Equals("All") || prop.Equals("Tree") || prop.Equals("True"));
            bool isBlockedSeed = tree.growthStage.Value == 0 && location.objects.ContainsKey(position);
            return tileCanSpawnTree && !isBlockedSeed;
        }


        public static bool ExperiencingWinter(this GameLocation location)
        {
            return Game1.IsWinter && location.ExperiencesWinter();
        }


        public static bool ExperiencesWinter(this GameLocation location)
        {
            return location.IsOutdoors && !(location is Desert);
        }


        public static bool IsShadedAt(this GameLocation location, Vector2 position)
        {
            foreach (Vector2 adjacentTile in Utility.getSurroundingTileLocationsArray(position))
            {
                if (location.terrainFeatures.TryGetValue(adjacentTile, out TerrainFeature feature)
                    && feature is Tree adjTree
                    && adjTree.IsFullyGrown()
                    && !adjTree.stump.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsFullyGrown(this Tree tree)
        {
            return tree.growthStage.Value >= Tree.treeStage;
        }

        public static bool IsMushroomTree(this Tree tree)
        {
            return tree.treeType.Value == Tree.mushroomTree;
        }
    }
}
