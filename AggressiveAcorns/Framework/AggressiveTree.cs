using System.Reflection;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.GameData.WildTrees;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;

namespace Phrasefable.StardewMods.AggressiveAcorns.Framework
{
    internal static class AggressiveTree
    {
        private static readonly MethodInfo methodInfo_setSeason = typeof(Tree).GetMethod("setSeason", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static readonly int MaxGrowthStage = Tree.stageForMossGrowth + 1;


        public static void DayUpdateAggressively(this Tree tree)
        {

            tree.TryRevertTempGreenRain();
            tree.wasShakenToday.Value = false;
            methodInfo_setSeason.Invoke(tree, null);
            tree.CheckForNewTexture();
            WildTreeData data = tree.GetData();



            // TODO check if there is any need to do the skip-update-of-first-day-when-spread thing
            bool isDestroyed = tree.DestroyIfDead();
            if (isDestroyed) return;

            if (tree.tapped.Value) tree.UpdateTapping();
            tree.FixRotation();


            tree.TryIncreaseStage();
            tree.ManageHibernation();


            // if (tree.TreeCanGrow())
            // {
            //     tree.PopulateSeed();
            //     tree.TrySpread();
            //     tree.TryIncreaseStage();
            //     tree.ManageHibernation();
            //     tree.TryRegrow();
            // }
        }


        public static int GetMaxSizeHereAggressively(this Tree tree, bool ignoreSeason = false)
        {
            if (tree.GetData() == null)
                return tree.growthStage.Value;

            if (tree.Location.IsNoSpawnTile(tree.Tile, "Tree") && !tree.Location.doesEitherTileOrTileIndexPropertyEqual((int) tree.Tile.X, (int) tree.Tile.Y, "CanPlantTrees", "Back", "T"))
                return tree.growthStage.Value;

            if (!ignoreSeason && !tree.IsInSeason()) // TODO determine season override approach. note that tree.IsInSeason() checks fertilized
                return tree.growthStage.Value;

            if (tree.growthStage.Value == 0 && tree.Location.objects.ContainsKey(tree.Tile))
                return 0;

            if (tree.IsGrowthBlockedByNearbyTree())
                return AggressiveAcorns.Config.MaxShadedGrowthStage;

            return 15;
        }


        public static bool IsGrowthBlockedByNearbyTreeAggressively(this Tree tree)
        {
            foreach (Vector2 adjacentTile in Utility.getSurroundingTileLocationsArray(tree.Tile))
            {
                if (tree.Location.terrainFeatures.TryGetValue(adjacentTile, out TerrainFeature feature)
                    && feature is Tree adjTree
                    && adjTree.IsFullyGrown()
                    && !adjTree.stump.Value)
                {
                    return true;
                }
            }

            return false;
        }


        public static IEnumerable<Vector2> GenerateSpreadOffsets()
        {
            if (!AggressiveAcorns.Config.RollForSpread) yield break;

            static int GetOffset() => Game1.random.Next(-3, 4);
            yield return new Vector2(GetOffset(), GetOffset());
        }


        // =============================================================================================================


        private static void TryRevertTempGreenRain(this Tree tree)
        {
            if (!tree.isTemporaryGreenRainTree.Value) return;
            if (Game1.IsFall || Game1.IsWinter) return;
            if (tree.Location is not null && tree.Location.IsGreenRainingHere()) return;

            tree.isTemporaryGreenRainTree.Value = false;
            tree.treeType.Value = tree.treeType.Value switch
            {
                Tree.greenRainTreeBushy => Tree.bushyTree,
                _ => Tree.leafyTree,
            };
            tree.resetTexture();
        }

        private static bool DestroyIfDead(this Tree tree)
        {
            if (tree.health.Value > -100) return false;

            tree.destroy.Value = true;
            return true;
        }


        private static void FixRotation(this Tree tree)
        {
            tree.shakeRotation = 0.0f;
        }


        private static void UpdateTapping(this Tree tree)
        {
            StardewValley.Object objectAtTile = tree.Location.getObjectAtTile((int) tree.Tile.X, (int) tree.Tile.Y);
            if (objectAtTile == null || !objectAtTile.IsTapper())
            {
                tree.tapped.Value = false;
            }
            else if (objectAtTile.heldObject.Value is null)
            {
                tree.UpdateTapperProduct(objectAtTile);
            }
        }


        private static void TryIncreaseStage(this Tree tree)
        {
            int maxStageHere = tree.GetMaxSizeHere();
            if (tree.growthStage.Value >= maxStageHere) return;


            //if (tree.IsFullyGrown() ||
            //    (tree.growthStage.Value >= AggressiveAcorns.Config.MaxShadedGrowthStage &&
            //     tree.Location.IsShadedAt(tree.Tile)))
            //{
            //    return;
            //}

            //// Trees experiencing winter won't grow unless fertilized or set to ignore winter.
            //// In addition to this, mushroom trees won't grow if they should be hibernating, even if fertilized.
            //if (tree.Location.ExperiencingWinter()
            //    && ((tree.IsMushroomTree() && AggressiveAcorns.Config.DoMushroomTreesHibernate)
            //        || !(AggressiveAcorns.Config.DoGrowInWinter || tree.fertilized.Value)))
            //{
            //    return;
            //}

            if (AggressiveAcorns.Config.DoGrowInstantly)
            {
                tree.growthStage.Value = maxStageHere;
            }
            else
            {
                // TODO genericize how specific tree types may have differing growth rates
                bool allowGrowth = tree.treeType.Value switch
                {
                    Tree.mahoganyTree => tree.fertilized.Value
                        ? AggressiveAcorns.Config.RollForGrowthMahoganyFertilized
                        : AggressiveAcorns.Config.RollForGrowthMahogany,
                    _ => tree.fertilized.Value || AggressiveAcorns.Config.RollForGrowth
                };

                if (allowGrowth) tree.growthStage.Value += 1;
            }
        }


        private static void ManageHibernation(this Tree tree)
        {
            if (!tree.IsMushroomTree()
                || !AggressiveAcorns.Config.DoMushroomTreesHibernate
                || !tree.Location.ExperiencesWinter())
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
                tree.RegrowStumpIfNotShaded();
            }
        }


        private static void TryRegrow(this Tree tree)
        {
            if (tree.IsMushroomTree() &&
                AggressiveAcorns.Config.DoMushroomTreesRegrow &&
                tree.stump.Value &&
                (!tree.Location.ExperiencingWinter() || (!AggressiveAcorns.Config.DoMushroomTreesHibernate &&
                                                    AggressiveAcorns.Config.DoGrowInWinter)) &&
                (AggressiveAcorns.Config.DoGrowInstantly || AggressiveAcorns.Config.RollForMushroomRegrowth))
            {
                tree.RegrowStumpIfNotShaded();
            }
        }


        private static void RegrowStumpIfNotShaded(this Tree tree)
        {
            if (tree.Location.IsShadedAt(tree.Tile)) return;

            tree.stump.Value = false;
            tree.health.Value = Tree.startingHealth;
        }


        private static void TrySpread(this Tree tree)
        {
            if (tree.Location is not Farm ||
                !tree.IsFullyGrown() ||
                (Game1.IsWinter && !AggressiveAcorns.Config.DoSpreadInWinter) ||
                (tree.tapped.Value && !AggressiveAcorns.Config.DoTappedSpread) ||
                tree.stump.Value)
            {
                return;
            }

            foreach (Vector2 offset in AggressiveAcorns.Config.SpreadSeedOffsets)
            {
                Vector2 seedPos = tree.Tile + offset;
                int tileX = (int) seedPos.X;
                int tileY = (int) seedPos.Y;
                if (AggressiveAcorns.Config.DoSeedsReplaceGrass &&
                    tree.Location.terrainFeatures.TryGetValue(seedPos, out TerrainFeature feature) &&
                    feature is Grass)
                {
                    tree.PlaceOffspring(seedPos);
                }
                else if (tree.Location.isTileLocationOpen(new Location(tileX, tileY))
                         && !tree.Location.IsTileOccupiedBy(seedPos)
                         && tree.Location.doesTileHaveProperty(tileX, tileY, "Water", "Back") == null
                         && tree.Location.isTileOnMap(seedPos))
                {
                    tree.PlaceOffspring(seedPos);
                }
            }
        }


        private static void PlaceOffspring(this Tree tree, Vector2 seedPosition)
        {
            var seed = new Tree(tree.treeType.Value, 0);
            tree.Location.terrainFeatures[seedPosition] = seed;
        }


        private static void PopulateSeed(this Tree tree)
        {
            if (!tree.IsFullyGrown() || tree.stump.Value) return;

            bool gainSeed = tree.treeType.Value switch
            {
                Tree.palmTree2 => AggressiveAcorns.Config.RollForSeedGain ||
                                  AggressiveAcorns.Config.RollForSeedGain ||
                                  AggressiveAcorns.Config.RollForSeedGain,
                _ => AggressiveAcorns.Config.RollForSeedGain
            };

            // Seed gain takes precedence over loss, hence loss is immaterial if it is just regained anyway
            if (gainSeed)
            {
                tree.hasSeed.Value = true;
            }
            else if (AggressiveAcorns.Config.RollForSeedLoss)
            {
                tree.hasSeed.Value = false;
            }
        }
    }
}
