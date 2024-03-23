using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Phrasefable.StardewMods.NoOre
{
    public class NoOre : Mod
    {
        private static IModConfig _config;

        private static IMonitor _monitor;

        private bool _handleNewLocations;

        private bool DoHandleNewLocations
        {
            get => this._handleNewLocations;
            set
            {
                if (value == this._handleNewLocations) return;
                if (value)
                {
                    this.Helper.Events.World.LocationListChanged += this.WorldOnLocationListChanged;
                }
                else
                {
                    this.Helper.Events.World.LocationListChanged -= this.WorldOnLocationListChanged;
                }

                this._handleNewLocations = value;
            }
        }


        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();
            _monitor = this.Monitor;

            var harmony = new Harmony(this.ModManifest.UniqueID);

            MethodInfo target = typeof(GameLocation).GetMethod("breakStone", BindingFlags.Instance | BindingFlags.NonPublic);

            // Monitor.Log(target != null ? $"got method info for {target.DeclaringType}::{target.Name}" : "couldn't reflect method",LogLevel.Trace);
            var postfix = new HarmonyMethod(this.GetType(), nameof(Postfix));
            harmony.Patch(target, null, postfix);

            helper.Events.GameLoop.SaveLoaded += (sender, args) => this.DoHandleNewLocations = Context.IsMainPlayer;
            helper.Events.GameLoop.ReturnedToTitle += (sender, args) => this.DoHandleNewLocations = false;
        }

        private void WorldOnLocationListChanged(object sender, LocationListChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }


        private static void Postfix(
#pragma warning disable IDE0060 // Remove unused parameter
            GameLocation __instance,
            bool __result,
            string stoneId,
            int x,
            int y,
            Farmer who
#pragma warning restore IDE0060 // Remove unused parameter
        )
        {
            // _monitor.Log(
            //     $"breakStone called in {__instance.Name} at ({x},{y}) by {who.Name} on {indexOfStone}",
            //     LogLevel.Trace);
            if (__result) return; // if the original method returned true, it wasn't normal stone

            if (_config.ReplaceOres)
            {
                // chance to drop ores
                /*
                 * Base chance options: flat chance, native range, by deepest level, by skill level
                 * Copper
                 * Iron
                 * Gold
                 * Iridium
                 */
            }

            if (_config.ReplaceGemNodes)
            {
                // chance to drop gems
            }

            if (_config.ReplaceMysticStone)
            {
                // chance to drop mystic stone drops
            }

            if (_config.ReplaceGeodeNodes)
            {
                // chance to drop geodes
            }
        }
    }


    internal static class Constants
    {
        public const int CopperNode = 378;
        public const int IronNode = 380;
        public const int CoalNode = 382;
        public const int GoldNode = 384;
        public const int IridiumNode = 386;
    }
}
