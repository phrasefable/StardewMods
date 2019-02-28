using Harmony;

namespace PhraseLib
{
    public interface IHarmonyPatch
    {
        void ApplyPatch(HarmonyInstance harmony);

        bool IsValid(HarmonyInstance harmony, out string errors);
    }
}