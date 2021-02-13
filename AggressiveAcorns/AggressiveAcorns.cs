using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns
{
    [UsedImplicitly]
    public class AggressiveAcorns : Mod
    {
        internal static IReflectionHelper ReflectionHelper;
        internal static IConfigAdaptor Config;


        private bool _manageTrees;

        private bool ManageTrees
        {
            get => _manageTrees;
            set
            {
                if (value == _manageTrees) return;

                Monitor.Log($"{(value ? "Started" : "Stopped")} watching for new trees/new areas.");
                _manageTrees = value;
            }
        }


        public override void Entry([NotNull] IModHelper helper)
        {
            AggressiveAcorns.Config = new ConfigAdaptor(helper.ReadConfig<ModConfig>());
            AggressiveAcorns.ReflectionHelper = helper.Reflection;

            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.World.LocationListChanged += OnLocationListChanged;
            helper.Events.World.TerrainFeatureListChanged += OnTerrainFeatureListChanged;
            helper.Events.GameLoop.Saving += OnSaving;
        }


        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            Monitor.Log("Enraging trees in all available areas.");
            ReplaceTerrainFeatures<Tree, AggressiveTree>(
                AggressiveAcorns.EnrageTree,
                Common.Utilities.GetLocations(Helper)
            );
            ManageTrees = true;
        }


        private void OnSaving(object sender, SavingEventArgs e)
        {
            ManageTrees = false;
            Monitor.Log("Calming trees in all available areas.");
            ReplaceTerrainFeatures<AggressiveTree, Tree>(
                AggressiveAcorns.CalmTree,
                Common.Utilities.GetLocations(Helper)
            );
        }


        private void OnLocationListChanged(object sender, LocationListChangedEventArgs e)
        {
            if (!ManageTrees) return;
            Monitor.Log("Found new areas; enraging any trees.");
            ReplaceTerrainFeatures<Tree, AggressiveTree>(AggressiveAcorns.EnrageTree, e.Added);
        }


        private void OnTerrainFeatureListChanged(object sender, TerrainFeatureListChangedEventArgs e)
        {
            // NOTE: this causes changes to the terrain feature list, make sure that this doesn't get stuck forever.
            if (!ManageTrees) return;

            IList<KeyValuePair<Vector2, Tree>> toReplace = AggressiveAcorns.GetTerrainFeatures<Tree>(e.Added);
            if (!toReplace.Any()) return;

            string msg = AggressiveAcorns.ReplaceTerrainFeatures(AggressiveAcorns.EnrageTree, e.Location, toReplace);
            Monitor.Log("TerrainFeature list changed: " + msg);
        }


        [NotNull]
        private static AggressiveTree EnrageTree([NotNull] Tree tree)
        {
            return new AggressiveTree(tree);
        }


        [NotNull]
        private static Tree CalmTree([NotNull] AggressiveTree tree)
        {
            return tree.ToTree();
        }


        private void ReplaceTerrainFeatures<TOriginal, TReplacement>(
            Func<TOriginal, TReplacement> converter,
            [NotNull] IEnumerable<GameLocation> locations)
            where TReplacement : TerrainFeature
            where TOriginal : TerrainFeature
        {
            foreach (GameLocation location in locations)
            {
                IList<KeyValuePair<Vector2, TOriginal>> toReplace =
                    AggressiveAcorns.GetTerrainFeatures<TOriginal>(location.terrainFeatures.Pairs);
                if (toReplace.Any())
                {
                    Monitor.Log(AggressiveAcorns.ReplaceTerrainFeatures(converter, location, toReplace));
                }
            }
        }


        [NotNull]
        private static IList<KeyValuePair<Vector2, T>> GetTerrainFeatures<T>(
            [NotNull] IEnumerable<KeyValuePair<Vector2, TerrainFeature>> items) where T : TerrainFeature
        {
            return items
                .Where(kvp => kvp.Value.GetType() == typeof(T))
                .Select(kvp => new KeyValuePair<Vector2, T>(kvp.Key, kvp.Value as T))
                .ToList();
        }


        [NotNull]
        private static string ReplaceTerrainFeatures<TOriginal, TReplacement>(
            Func<TOriginal, TReplacement> converter,
            [NotNull] GameLocation location,
            [NotNull] ICollection<KeyValuePair<Vector2, TOriginal>> terrainFeatures)
            where TReplacement : TerrainFeature
            where TOriginal : class
        {
            foreach (KeyValuePair<Vector2, TOriginal> keyValuePair in terrainFeatures)
            {
                location.terrainFeatures[keyValuePair.Key] = converter(keyValuePair.Value);
            }

            return
                $"{location.Name} - replaced {terrainFeatures.Count} {typeof(TOriginal).Name} with {typeof(TReplacement).Name}.";
        }
    }
}
