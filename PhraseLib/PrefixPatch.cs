using System.Reflection;
using Harmony;

namespace PhraseLib {

    public abstract class PrefixPatch : HarmonyPatch {
        private const BindingFlags PatchBindingFlags = BindingFlags.NonPublic | BindingFlags.Static;

        protected abstract string PatchMethod { get; }


        public sealed override void ApplyPatch(HarmonyInstance harmony) {
            harmony.Patch(GetTargetMethod(), GetPatch());
        }


        private HarmonyMethod GetPatch() {
            return new HarmonyMethod(GetType().GetMethod(PatchMethod, PatchBindingFlags));
        }
    }

}