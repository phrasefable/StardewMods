using System.Collections.Generic;
using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace PhraseLib
{
    public abstract class PatchingMod : Mod
    {
        private IEnumerable<IHarmonyPatch> _patches = new List<IHarmonyPatch>();

        protected void Patch(IModHelper helper)
        {
            _patches = LoadPatches();
            ApplyPatches();
            helper.Events.GameLoop.GameLaunched += ValidatePatches;
        }

        protected abstract IEnumerable<IHarmonyPatch> LoadPatches();

        private void ApplyPatches()
        {
            var harmony = HarmonyInstance.Create(ModManifest.UniqueID);
            foreach (var patch in _patches)
            {
                patch.ApplyPatch(harmony);
            }
        }

        private void ValidatePatches(object sender, GameLaunchedEventArgs e)
        {
            var harmony = HarmonyInstance.Create(ModManifest.UniqueID);
            var firstError = true;
            foreach (var patch in _patches)
            {
                if (patch.IsValid(harmony, out var errors)) continue;

                if (firstError)
                {
                    Monitor.Log("You have conflicting mods. Please check whether they both change the same thing.");
                    firstError = false;
                }

                Monitor.Log(errors, LogLevel.Error);
            }
        }
    }
}