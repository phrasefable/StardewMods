using StardewModdingAPI;
using StardewValley;

namespace Phrasefable.StardewMods.ModdingTools
{
    public partial class PhrasefableModdingTools
    {
        private void SetUp_Clear()
        {
            const string doc = "clears all objects and terrain features from current location";
            this.Helper.ConsoleCommands.Add("clear_map", doc, this.ClearGround);
        }


        private void ClearGround(string arg1, string[] arg2)
        {
            if (Context.IsWorldReady)
            {
                GameLocation location = Game1.currentLocation;
                location.debris.Clear();
                location.objects.Clear();
                location.terrainFeatures.Clear();
                location.largeTerrainFeatures.Clear();
            }
            else
            {
                this.Monitor.Log("World not ready", LogLevel.Info);
            }
        }
    }
}