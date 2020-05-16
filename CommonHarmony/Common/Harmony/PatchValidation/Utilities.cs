using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using StardewModdingAPI;

namespace Common.Harmony.PatchValidation
{
    internal static class Utilities
    {
        public static void ValidatePatches(
            HarmonyInstance harmony,
            IEnumerable<IHarmonyPatchInfo> patches,
            IMonitor monitor,
            IModRegistry registry)
        {
            List<ValidationResult> invalidResults = patches
                .Select(patch => patch.IsValid(harmony, BuildModLookup(registry)))
                .Where(result => !result.IsValid)
                .ToList();

            if (!invalidResults.Any()) return;

            monitor.Log(
                "Harmony patches failed validation:\n",
                LogLevel.Warn
            );
            invalidResults.ForEach(
                r => r.Information.ForEach(
                    line => monitor.Log(line, LogLevel.Warn)
                )
            );
        }


        private static Func<string, string> BuildModLookup(IModRegistry registry)
        {
            var cache = new Dictionary<string, string>();
            string Lookup(string modId) => registry.IsLoaded(modId) ? registry.Get(modId).Manifest.Name : modId;

            return modId =>
            {
                if (!cache.TryGetValue(modId, out string modName))
                {
                    modName = Lookup(modId == "this" ? registry.ModID : modId);
                    cache[modId] = modName;
                }

                return modName;
            };
        }
    }
}