using System.Text;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

using SdvObject = StardewValley.Object;

namespace Phrasefable.StardewMods.ModdingTools
{
    public partial class PhrasefableModdingTools
    {
        private ToggleableEventHandler<WarpedEventArgs> _tallyHandler;


        private void SetUp_Tally()
        {
            this._tallyHandler = new ToggleableEventHandler<WarpedEventArgs>(this.Tally);
            this.Helper.Events.Player.Warped += this._tallyHandler.OnEvent;

            var desc = new StringBuilder("Counts the objects in the current location.");
            desc.AppendLine("Usage: count_objects [all|start|stop]");
            desc.AppendLine("    all   - count the objects in every location");
            desc.AppendLine("    start - start counting each time a location is entered");
            desc.Append("    stop  - stop counting each time a location is entered");
            this.Helper.ConsoleCommands.Add("count_objects", desc.ToString(), this.TallyObjectCommand);
            this.Helper.ConsoleCommands.Add("count_terrain", "counts terrain features", this.CountTerrainFeatures);
        }


        private void Tally(object sender, WarpedEventArgs e)
        {
            if (e.IsLocalPlayer) this.CountObjects(e.NewLocation);
        }


        private void TallyObjectCommand(string command, string[] args)
        {
            if (args.Length == 0)
            {
                this.CountObjects();
            }
            else
            {
                switch (args[0])
                {
                    case "start":
                        this._tallyHandler.Set(ToggleAction.Enable);
                        break;
                    case "stop":
                        this._tallyHandler.Set(ToggleAction.Disable);
                        break;
                    case "all":
                        this.CountObjects(true);
                        break;
                    default:
                        this.Monitor.Log($"Arguments `{string.Join(" ", args)}` malformed.", LogLevel.Info);
                        break;
                }
            }
        }


        private void CountTerrainFeatures(string arg1, string[] arg2)
        {
            if (Context.IsWorldReady)
            {
                this.CountTerrainFeatures(Game1.currentLocation);
            }
            else
            {
                this.Monitor.Log("World not ready", LogLevel.Info);
            }
        }


        private void CountObjects(bool allLocations = false)
        {
            if (!Context.IsWorldReady)
            {
                this.Monitor.Log("World not ready", LogLevel.Info);
                return;
            }

            if (allLocations)
            {
                foreach (GameLocation location in Common.Utilities.GetLocations(this.Helper)) this.CountObjects(location);
            }
            else
            {
                this.CountObjects(Game1.currentLocation);
            }
        }


        private void CountObjects(GameLocation location)
        {
            IEnumerable<List<SdvObject>> results = from obj in location.objects.Values
                                                   group obj by obj.ParentSheetIndex
                                                   into grouping
                                                   orderby grouping.Key
                                                   select grouping.ToList();

            this.Monitor.Log($"Counted objects in {location.Name}:", LogLevel.Info);
            foreach (List<SdvObject> objects in results)
            {
                SdvObject first = objects.First();
                this.Monitor.Log($"    {first.ParentSheetIndex} {first.DisplayName} - {objects.Count}", LogLevel.Info);
            }
        }


        private void CountTerrainFeatures(GameLocation location)
        {
            var results = from feat in location.terrainFeatures.Values
                          group feat by feat.GetType()
                          into grouping
                          orderby grouping.Key.Name
                          select new { grouping.Key.Name, Count = grouping.Count() };

            this.Monitor.Log($"Counted terrain features in {location.Name}", LogLevel.Info);
            foreach (var result in results)
            {
                this.Monitor.Log($"    {result.Name} - {result.Count}");
            }
        }


        // todo make some sort of command that will rapidly warp through all mine floors.
        // todo add name filter?
    }
}
