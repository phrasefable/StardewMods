using System.Text;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Phrasefable.StardewMods.ModdingTools
{
    public partial class PhrasefableModdingTools
    {
        private readonly ToggleableEventLoggerCollection _loggers = new ToggleableEventLoggerCollection();
        private readonly string[] _enable = { "1", "true", "t" };
        private readonly string[] _disable = { "0", "false", "f" };


        private void SetUp_EventLogging()
        {
            this.BuildLogger_World_DebrisListChanged();
            this.BuildLogger_World_ObjectListChanged();
            this.BuildLogger_World_LocationListChanged();
            this.BuildLogger_World_TerrainFeatureListChanged();
            this.BuildLogger_World_LargeTerrainFeatureListChanged();
            this.BuildLogger_GameLoop_Saving();
            this.BuildLogger_GameLoop_Saved();
            this.BuildLogger_GameLoop_SaveLoaded();
            this.BuildLogger_GameLoop_DayStarted();
            this.BuildLogger_GameLoop_DayEnding();

            const string desc = "Usage: log_events [event...][ {1|true|t|0|false|f}]";
            this.Helper.ConsoleCommands.Add("log_events", desc, this.Callback);
        }


        private void Callback(string command, [NotNull] string[] args)
        {
            var action = ToggleAction.Toggle;
            var targets = new List<string>();
            List<string> validIds = this._loggers.Ids.ToList();

            foreach (string arg in args)
            {
                if (this._enable.Contains(arg))
                {
                    action = ToggleAction.Enable;
                }
                else if (this._disable.Contains(arg))
                {
                    action = ToggleAction.Disable;
                }
                else if (action == ToggleAction.Toggle && validIds.Contains(arg))
                {
                    targets.Add(arg);
                }
                else
                {
                    this.Monitor.Log($"Argument '{arg}' malformed. Command aborted.");
                    return;
                }
            }

            if (targets.Any())
            {
                this._loggers.Set(targets, action);
            }
            else if (action != ToggleAction.Toggle)
            {
                targets = validIds;
                this._loggers.Set(targets, action);
            }

            var message = new StringBuilder("Enabled:");
            foreach (IToggleableEventLogger logger in this._loggers.Where(l => l.IsEnabled).OrderBy(l => l.Id))
            {
                message.Append($" {(targets.Contains(logger.Id) ? "*" : "")}{logger.Id}");
            }

            this.Monitor.Log(message.ToString(), LogLevel.Info);

            message = new StringBuilder("Disabled:");
            foreach (IToggleableEventLogger logger in this._loggers.Where(l => !l.IsEnabled).OrderBy(l => l.Id))
            {
                message.Append($" {(targets.Contains(logger.Id) ? "*" : "")}{logger.Id}");
            }

            this.Monitor.Log(message.ToString(), LogLevel.Info);
        }


        [NotNull]
        private ToggleableEventLogger<T> BuildLogger<T>([NotNull] string name, Func<T, string> message)
            where T : EventArgs
        {
            var logger = new ToggleableEventLogger<T>(name, this.Monitor, message);
            this._loggers.Add(logger);
            return logger;
        }


        private void BuildLogger_World_DebrisListChanged()
        {
            ToggleableEventLogger<DebrisListChangedEventArgs> logger = this.BuildLogger(
                "debris",
                (DebrisListChangedEventArgs args) =>
                    $"World.DebrisListChanged {args.Location.Name} +{args.Added.Count()} -{args.Removed.Count()}"
            );
            this.Helper.Events.World.DebrisListChanged += logger.OnEvent;
        }


        private void BuildLogger_World_ObjectListChanged()
        {
            ToggleableEventLogger<ObjectListChangedEventArgs> logger = this.BuildLogger(
                "objects",
                (ObjectListChangedEventArgs args) =>
                    $"World.ObjectListChanged {args.Location.Name} +{args.Added.Count()} -{args.Removed.Count()}"
            );
            this.Helper.Events.World.ObjectListChanged += logger.OnEvent;
        }


        private void BuildLogger_World_LocationListChanged()
        {
            ToggleableEventLogger<LocationListChangedEventArgs> logger = this.BuildLogger(
                "locations",
                (LocationListChangedEventArgs args) =>
                    $"World.LocationListChanged +{args.Added.Count()} -{args.Removed.Count()}"
            );
            this.Helper.Events.World.LocationListChanged += logger.OnEvent;
        }


        private void BuildLogger_World_TerrainFeatureListChanged()
        {
            ToggleableEventLogger<TerrainFeatureListChangedEventArgs> logger = this.BuildLogger(
                "terrain",
                (TerrainFeatureListChangedEventArgs args) =>
                    $"World.TerrainFeatureListChanged {args.Location.Name} +{args.Added.Count()} -{args.Removed.Count()}"
            );
            this.Helper.Events.World.TerrainFeatureListChanged += logger.OnEvent;
        }


        private void BuildLogger_World_LargeTerrainFeatureListChanged()
        {
            ToggleableEventLogger<LargeTerrainFeatureListChangedEventArgs> logger = this.BuildLogger(
                "large_terrain",
                (LargeTerrainFeatureListChangedEventArgs args) =>
                    $"World.LargeTerrainFeatureListChanged {args.Location.Name} +{args.Added.Count()} -{args.Removed.Count()}"
            );
            this.Helper.Events.World.LargeTerrainFeatureListChanged += logger.OnEvent;
        }


        private void BuildLogger_GameLoop_Saving()
        {
            ToggleableEventLogger<SavingEventArgs> logger = this.BuildLogger(
                "saving",
                (SavingEventArgs args) => "GameLoop.Saving"
            );
            this.Helper.Events.GameLoop.Saving += logger.OnEvent;
        }


        private void BuildLogger_GameLoop_Saved()
        {
            ToggleableEventLogger<SavedEventArgs> logger = this.BuildLogger(
                "saved",
                (SavedEventArgs args) => "GameLoop.Saved"
            );
            this.Helper.Events.GameLoop.Saved += logger.OnEvent;
        }


        private void BuildLogger_GameLoop_SaveLoaded()
        {
            ToggleableEventLogger<SaveLoadedEventArgs> logger = this.BuildLogger(
                "save_loaded",
                (SaveLoadedEventArgs args) => "GameLoop.SaveLoaded"
            );
            this.Helper.Events.GameLoop.SaveLoaded += logger.OnEvent;
        }


        private void BuildLogger_GameLoop_DayStarted()
        {
            ToggleableEventLogger<DayStartedEventArgs> logger = this.BuildLogger(
                "day_started",
                (DayStartedEventArgs args) => "GameLoop.DayStarted"
            );
            this.Helper.Events.GameLoop.DayStarted += logger.OnEvent;
        }


        private void BuildLogger_GameLoop_DayEnding()
        {
            ToggleableEventLogger<DayEndingEventArgs> logger = this.BuildLogger(
                "day_ending",
                (DayEndingEventArgs args) => "GameLoop.DayEnding"
            );
            this.Helper.Events.GameLoop.DayEnding += logger.OnEvent;
        }
    }
}
