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
            SetUp_Ground();

            SetUp_Tally();

            SetUp_EventLogging();

            SetUp_Clear();

            SetUp_Misc();
        }

        private void SetUp_Misc()
        {
            Helper.ConsoleCommands.Add(
                "list_game_locations",
                "Lists all game locations",
                ListGameLocations
            );
        }

        private void ListGameLocations(string arg1, string[] arg2)
        {
            if (!Context.IsWorldReady)
            {
                Monitor.Log("World not ready.", LogLevel.Info);
                return;
            }

            Monitor.Log("(Currently loaded) Game Locations:", LogLevel.Info);
            foreach (GameLocation location in Common.Utilities.GetLocations(Helper))
            {
                Monitor.Log($"  name={location.Name}", LogLevel.Info);
            }
        }
    }
}
