using Harmony;

namespace Common.Harmony.PatchTypes
{
    internal class PostfixPatch : IPatchApplicator
    {
        public static readonly PostfixPatch Instance = new PostfixPatch();
        private PostfixPatch() { }


        public void ApplyPatch(HarmonyInstance harmony, IHarmonyPatchInfo harmonyPatch)
        {
            harmony.Patch(harmonyPatch.PatchTarget, postfix: new HarmonyMethod(harmonyPatch.PatchSource));
        }
    }
}