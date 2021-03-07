using System.Reflection;

namespace Phrasefable.StardewMods.Common.Harmony
{
    internal class HarmonyPatchInfo : IHarmonyPatchInfo
    {
        public HarmonyPatchInfo(MethodInfo patchTarget, MethodInfo patchSource, PatchType patchType)
        {
            this.PatchTarget = patchTarget;
            this.PatchSource = patchSource;
            this.PatchType = patchType;
        }

        public MethodInfo PatchTarget { get; }
        public MethodInfo PatchSource { get; }
        public PatchType PatchType { get; }
    }
}
