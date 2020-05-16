using Harmony;

namespace Common.Harmony.PatchTypes
{
    internal interface IPatchApplicator
    {
        void ApplyPatch(HarmonyInstance harmony, IHarmonyPatchInfo harmonyPatch);
    }
}