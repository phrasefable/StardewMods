using Common.Harmony.PatchTypes;
using Harmony;

namespace Common.Harmony
{
    internal class PrefixPatch : IPatchApplicator
    {
        public static readonly PrefixPatch Instance = new PrefixPatch();
        private PrefixPatch() { }


        public void ApplyPatch(HarmonyInstance harmony, IHarmonyPatchInfo harmonyPatch)
        {
            harmony.Patch(harmonyPatch.PatchTarget, new HarmonyMethod(harmonyPatch.PatchSource));
        }
    }
}