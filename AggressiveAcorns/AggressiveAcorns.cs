using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using Harmony;
using StardewModdingAPI.Events;

namespace AggressiveAcorns
{
    public class AggressiveAcorns : Mod
    {
        private IModConfig _config;
        
        private readonly List<IHarmonyPatch> _patches = new List<IHarmonyPatch>();

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();

            var harmony = HarmonyInstance.Create(ModManifest.UniqueID);

            LoadPatches();
            ApplyPatches(harmony);
            
            
            Helper.Events.GameLoop.GameLaunched += ValidatePatches;
        }

        private void ApplyPatches(HarmonyInstance harmony)
        {
            foreach (var patch in _patches)
            {
                patch.ApplyPatch(harmony);
            }
        }

        private void LoadPatches()
        {
            _patches.Add(new Patch_Tree_DayUpdate());
            if (_config.bPreventScythe)
            {
                _patches.Add(new Patch_Tree_PerformToolAction());
            }
        }

        private void ValidatePatches(object sender, GameLaunchedEventArgs e)
        {
            var harmony = HarmonyInstance.Create(ModManifest.UniqueID);
            foreach (var patch in _patches)
            {
                if (!patch.IsValid(harmony, out var errors))
                {
                    Monitor.Log(errors, LogLevel.Error);
                }
                
            }
        }
    }
}