using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using xTile.Dimensions;

namespace TestHarmony
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            HarmonyInstance.DEBUG = true;
            FileLog.Reset();

            var harmony = HarmonyInstance.Create("phrasefable.testharmony");

            var original = typeof(Tree).GetMethod("dayUpdate", new[] {typeof(GameLocation), typeof(Vector2)});
            var prefix = typeof(ModEntry).GetMethod("TreeDayUpdate", BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(original, new HarmonyMethod(prefix));

            original = typeof(Tree).GetMethod("performToolAction",
                new[] {typeof(Tool), typeof(int), typeof(Vector2), typeof(GameLocation)});
//            prefix = typeof(ModEntry).GetMethod("TreeToolAction", BindingFlags.Static | BindingFlags.NonPublic);
//            harmony.Patch(original, new HarmonyMethod(prefix));

            var transpiler =
                typeof(ModEntry).GetMethod("TreeToolActionTranspile", BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(original, null, null, new HarmonyMethod(transpiler));

            HarmonyInstance.DEBUG = false;
        }

        // ReSharper disable once UnusedMember.Local
        private static bool TreeDayUpdate(Tree __instance, GameLocation environment, Vector2 tileLocation)
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
                var canSpawn = environment.doesTileHaveProperty(tileX, (int) tileY, "NoSpawn", "Back");
                if (
                    (canSpawn == null || !canSpawn.Equals(nameof(Tree)) && !canSpawn.Equals("All") &&
                     !canSpawn.Equals("True"))
                    && (
                        environment.isTileLocationOpen(new Location(tileX * 64, tileY * 64))
                        && !environment.isTileOccupied(tile, "")
                        && (environment.doesTileHaveProperty(tileX, tileY, "Water", "Back") == null
                            && environment.isTileOnMap(tile))
                    )
                )
                    environment.terrainFeatures.Add(tile, new Tree(__instance.treeType.Value, 0));
            }


            return false;
        }

        // ReSharper disable once UnusedMember.Local
        private static bool TreeToolAction(Tree __instance, Tool t, ref bool __result)
        {
            if (t is MeleeWeapon)
            {
                __result = false;
                return false;
            }

            return true;
        }

        // ReSharper disable once UnusedMember.Local
        private static IEnumerable<CodeInstruction> TreeToolActionTranspile(MethodBase original,
            IEnumerable<CodeInstruction> instructions)
        {
            var found = false;
            var done = false;
            foreach (var instruction in instructions)
            {
                if (!done)
                {
                    if (found)
                    {
                        if (instruction.opcode.Equals(OpCodes.Brfalse))
                        {
                            FileLog.Log("++++++ Found opcode to change");
                            instruction.opcode = OpCodes.Brtrue;
                            done = true;
                        }
                        else
                        {
                            FileLog.Log("++++++ Could not find opcode to change.");
                            throw new Exception(
                                "Could patch tree tool action. Could not find the CIL instruction to change");
                        }
                    }
                    else if (instruction.opcode.Equals(OpCodes.Isinst) &&
                             instruction.operand.ToString().Equals("StardewValley.Tools.MeleeWeapon"))
                    {
                        FileLog.Log("++++++ Found indicator opcode");
                        found = true;
                    }
                }

                yield return instruction;
            }
        }
    }
}