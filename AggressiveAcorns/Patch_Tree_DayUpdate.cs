using System;
using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;

namespace AggressiveAcorns
{
    public class Patch_Tree_DayUpdate : PrefixPatch
    {
        protected override Type TargetType => typeof(Tree);

        protected override string TargetName => nameof(Tree.dayUpdate);

        protected override Type[] TargetParameters => new[]
        {
            typeof(GameLocation),
            typeof(Vector2)
        };

        protected override string PatchMethod => nameof(Prefix);

        public override bool IsValid(HarmonyInstance harmony, out string errors)
        {
            return IsExclusivePatch(harmony, out errors);
        }


        private static bool Prefix(Tree __instance, GameLocation environment, Vector2 tileLocation)
        {
            if (__instance.growthStage.Value == Tree.seedStage)
            {
                __instance.growthStage.Value = Tree.sproutStage;
                return false;
            }

            __instance.growthStage.Value = Tree.treeStage;

            // Place seeds on all surrounding clear tiles.
            var adjacentTiles = Utility.getAdjacentTileLocations(tileLocation);
            adjacentTiles.AddRange(Utility.getDiagonalTileLocationsArray(tileLocation));

            foreach (var tile in adjacentTiles)
            {
                var tileX = (int) tile.X;
                var tileY = (int) tile.Y;
                var canSpawn = environment.doesTileHaveProperty(tileX, tileY, "NoSpawn", "Back");
                if (
                    (canSpawn == null || !canSpawn.Equals(nameof(Tree)) && !canSpawn.Equals("All") &&
                     !canSpawn.Equals("True"))
                    && (
                        environment.isTileLocationOpen(new Location(tileX * 64, tileY * 64))
                        && !environment.isTileOccupied(tile)
                        && (environment.doesTileHaveProperty(tileX, tileY, "Water", "Back") == null
                            && environment.isTileOnMap(tile))
                    )
                )
                    environment.terrainFeatures.Add(tile, new Tree(__instance.treeType.Value, 0));
            }

            return false;
        }
    }
}