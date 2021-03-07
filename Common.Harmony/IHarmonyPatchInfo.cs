using System.Reflection;

namespace Phrasefable.StardewMods.Common.Harmony
{
    internal interface IHarmonyPatchInfo
    {
        MethodInfo PatchTarget { get; }
        MethodInfo PatchSource { get; }
        PatchType PatchType { get; }
    }
}
