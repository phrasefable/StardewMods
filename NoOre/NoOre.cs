﻿using System.Reflection;
using Harmony;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using SObject = StardewValley.Object;

namespace Phrasefable.StardewMods.NoOre
{
    [UsedImplicitly]
    public class NoOre : Mod
    {
        private static IModConfig _config;

        // ReSharper disable once NotAccessedField.Local
        private static IMonitor _monitor;

        private bool _handleNewLocations;

        private bool DoHandleNewLocations
        {
            // ReSharper disable once UnusedMember.Local
            get => _handleNewLocations;
            set
            {
                if (value == _handleNewLocations) return;
                if (value)
                {
                    Helper.Events.World.LocationListChanged += WorldOnLocationListChanged;
                }
                else
                {
                    Helper.Events.World.LocationListChanged -= WorldOnLocationListChanged;
                }

                _handleNewLocations = value;
            }
        }


        public override void Entry([NotNull] IModHelper helper)
        {
            NoOre._config = helper.ReadConfig<ModConfig>();
            NoOre._monitor = Monitor;

            var harmony = HarmonyInstance.Create(ModManifest.UniqueID);

            MethodInfo target = typeof(GameLocation).GetMethod("breakStone");

            // Monitor.Log(target != null ? $"got method info for {target.DeclaringType}::{target.Name}" : "couldn't reflect method",LogLevel.Trace);
            var postfix = new HarmonyMethod(GetType(), nameof(NoOre.Postfix));
            harmony.Patch(target, null, postfix);

            helper.Events.GameLoop.SaveLoaded += (sender, args) => DoHandleNewLocations = Context.IsMainPlayer;
            helper.Events.GameLoop.ReturnedToTitle += (sender, args) => DoHandleNewLocations = false;
        }


        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void WorldOnLocationListChanged(object sender, LocationListChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }


        private static void Postfix(
            // ReSharper disable UnusedParameter.Local
            GameLocation __instance,
            bool __result,
            int indexOfStone,
            int x,
            int y,
            Farmer who
            // ReSharper restore UnusedParameter.Local
        )
        {
            // _monitor.Log(
            //     $"breakStone called in {__instance.Name} at ({x},{y}) by {who.Name} on {indexOfStone}",
            //     LogLevel.Trace);
            if (__result) return; // if the original method returned true, it wasn't normal stone

            if (NoOre._config.ReplaceOres)
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

            if (NoOre._config.ReplaceGemNodes)
            {
                // chance to drop gems
            }

            if (NoOre._config.ReplaceMysticStone)
            {
                // chance to drop mystic stone drops
            }

            if (NoOre._config.ReplaceGeodeNodes)
            {
                // chance to drop geodes
            }
        }
    }


    // ReSharper disable UnusedType.Global, UnusedMember.Global
    internal static class Constants
    {
        public const int CopperNode = 378;
        public const int IronNode = 380;
        public const int CoalNode = 382;
        public const int GoldNode = 384;
        public const int IridiumNode = 386;
    }
    // ReSharper restore UnusedType.Global, UnusedMember.Global
}
