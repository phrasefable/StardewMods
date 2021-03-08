using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;

namespace Phrasefable.StardewMods.AggressiveAcorns.Framework
{
    internal static class AggressiveTree
    {
        public static void DayUpdateAggressively(
            this Tree tree,
            GameLocation location,
            Vector2 position,
            NetBool destroy)
        {
            // TODO check if there is any need to do the skip-update-of-first-day-when-spread thing
            bool isDestroyed = tree.DestroyIfDead(destroy);

            tree.ValidateTapped(location, position);

            if (!isDestroyed && location.TreeCanGrowAt(tree, position))
            {
                tree.PopulateSeed();
                tree.TrySpread(location, position);
                tree.TryIncreaseStage(location, position);
                tree.ManageHibernation(location, position);
                tree.TryRegrow(location, position);
            }
        }


        public static IEnumerable<Vector2> GenerateSpreadOffsets()
        {
            if (!AggressiveAcorns.Config.RollForSpread) yield break;

            static int GetOffset() => Game1.random.Next(-3, 4);
            yield return new Vector2(GetOffset(), GetOffset());
        }


        private static bool DestroyIfDead(this Tree tree, NetBool destroy)
        {
            if (tree.health.Value > -100) return false;

            destroy.Value = true;
            return true;
        }


        private static void ValidateTapped(this Tree tree, GameLocation environment, Vector2 tileLocation)
        {
            if (!tree.tapped.Value) return;

            Object objectAtTile = environment.getObjectAtTile((int) tileLocation.X, (int) tileLocation.Y);
            if (objectAtTile == null || !objectAtTile.bigCraftable.Value || objectAtTile.ParentSheetIndex != 105)
            {
                tree.tapped.Value = false;
            }
        }


        private static void TryIncreaseStage(this Tree tree, GameLocation location, Vector2 position)
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


        private static void ManageHibernation(this Tree tree, GameLocation location, Vector2 position)
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


        private static void TryRegrow(this Tree tree, GameLocation location, Vector2 position)
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


        private static void RegrowStumpIfNotShaded(this Tree tree, GameLocation location, Vector2 position)
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


        private static void TrySpread(this Tree tree, GameLocation location, Vector2 position)
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


        private static void PlaceOffspring(this Tree tree, GameLocation location, Vector2 seedPosition)
        {
            tree.hasSeed.Value = false;

            var seed = new Tree(tree.treeType.Value, 0);
            location.terrainFeatures[seedPosition] = seed;
        }


        private static void PopulateSeed(this Tree tree)
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
    }
}
