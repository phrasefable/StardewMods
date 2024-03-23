using System.Text;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Phrasefable.StardewMods.Common;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.ModdingTools
{
    public partial class PhrasefableModdingTools
    {
        private void SetUp_Ground()
        {
            var desc = new StringBuilder("Prints data on the tiles around the player.");
            desc.Append("Usage: ground [radius]");
            this.Helper.ConsoleCommands.Add("ground", desc.ToString(), this.GroundCommand);
        }


        private void GroundCommand(string command, string[] args)
        {
            if (!Context.IsWorldReady)
            {
                this.Monitor.Log("World not ready.", LogLevel.Info);
                return;
            }

            int radius = 1;
            if (args.Length > 0 && int.TryParse(args[0], out int value))
            {
                radius = value;
            }

            Vector2 playerPosition = Game1.player.Tile;

            foreach (Vector2 tile in Utilities.GetTilesInRadius(playerPosition, radius))
            {
                this.CheckGround(Game1.currentLocation, tile);
            }
        }


        private void CheckGround([NotNull] GameLocation location, Vector2 tile)
        {
            var message = new StringBuilder($"{location.Name} {tile}:");
            bool any = false;

            if (location.Objects.TryGetValue(tile, out StardewValley.Object sObject))
            {
                message.Append($" Object {sObject.Name} id {sObject.ParentSheetIndex}");
                if (sObject.IsSpawnedObject) message.Append(" [Spawned]");
                if (sObject.CanBeGrabbed) message.Append(" [CanBeGrabbed]");
                if (sObject.bigCraftable.Value) message.Append(" [BigCraftable]");
                message.Append(";");
                any = true;
            }

            if (location.terrainFeatures.TryGetValue(tile, out TerrainFeature feature))
            {
                message.Append($" Terrain feature {feature.GetType().FullName}");
                any = true;
            }

            if (!any)
            {
                message.Append(" (none)");
            }

            this.Monitor.Log(message.ToString(), LogLevel.Info);
        }
    }
}
