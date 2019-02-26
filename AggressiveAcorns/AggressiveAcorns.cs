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

            FileLog.Reset();
            HarmonyInstance.DEBUG = true;
            var harmony = HarmonyInstance.Create(ModManifest.UniqueID);

            var updatepatch = new Patch_Tree_DayUpdate();
            _patches.Add(updatepatch);
            if (_config.bPreventScythe)
            {
                _patches.Add(new Patch_Tree_PerformToolAction());
            }

            foreach (var patch in _patches)
            {
                patch.ApplyPatch(harmony);
            }
            
            
            Helper.Events.GameLoop.GameLaunched += ValidatePatches;
            HarmonyInstance.DEBUG = false;
            Monitor.Log("Loaded aggressive acorns.");
        }

        private void LogPatches(HarmonyInstance harmony)
        {
            foreach (var patchedMethod in harmony.GetPatchedMethods())
            {
                Monitor.Log($"Found patched method: {patchedMethod.DeclaringType}::{patchedMethod.Name}");
                var patches = harmony.GetPatchInfo(patchedMethod);
                Monitor.Log("  Has prefixes:");
                foreach (var prefix in patches.Prefixes)
                {
                    Monitor.Log($"    Owner: {prefix.owner}; method: {prefix.patch.Name}");
                }

                Monitor.Log("  Has postfixes:");
                foreach (var postfix in patches.Postfixes)
                {
                    Monitor.Log($"    Owner: {postfix.owner}; method: {postfix.patch.Name}");
                }

                Monitor.Log("  Has transpilers:");
                foreach (var trans in patches.Transpilers)
                {
                    Monitor.Log($"    Owner: {trans.owner}; method: {trans.patch.Name}");
                }
            }
        }

        private void ValidatePatches(object sender, GameLaunchedEventArgs e)
        {
            var harmony = HarmonyInstance.Create(ModManifest.UniqueID);
            LogPatches(harmony);
            Monitor.Log("Validating patches.");
            foreach (var patch in _patches)
            {
                Monitor.Log($"Validating patch {patch.GetType()}");
                if (!patch.IsValid(harmony, out var errors))
                {
                    Monitor.Log(errors, LogLevel.Error);
                }
                
            }
        }
    }
}