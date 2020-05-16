using System;
using System.Reflection;
using Common.Harmony.PatchValidation;
using Harmony;

namespace Common.Harmony
{
    internal interface IHarmonyPatchInfo
    {
        MethodInfo PatchTarget { get; }
        MethodInfo PatchSource { get; }

        void ApplyPatch(HarmonyInstance harmony);
        ValidationResult IsValid(HarmonyInstance harmony, Func<string, string> modLookup);
    }
}