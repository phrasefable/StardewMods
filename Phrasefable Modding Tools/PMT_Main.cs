using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;

namespace Phrasefable.StardewMods.ModdingTools
{
    [UsedImplicitly]
    public partial class PhrasefableModdingTools : Mod
    {
        public override void Entry([NotNull] IModHelper helper)
        {
            this.SetUp_Ground();

            this.SetUp_Tally();

            this.SetUp_EventLogging();

            this.SetUp_Clear();

            this.SetUp_Misc();
        }

        private void SetUp_Misc()
        {
            this.Helper.ConsoleCommands.Add(
                "list_game_locations",
                "Lists all game locations",
                this.ListGameLocations
            );
        }

        private void ListGameLocations(string arg1, string[] arg2)
        {
            if (!Context.IsWorldReady)
            {
                this.Monitor.Log("World not ready.", LogLevel.Info);
                return;
            }

            this.Monitor.Log("(Currently loaded) Game Locations:", LogLevel.Info);
            foreach (GameLocation location in Common.Utilities.GetLocations(this.Helper))
            {
                this.Monitor.Log($"  name={location.Name}", LogLevel.Info);
            }
        }
    }
}
