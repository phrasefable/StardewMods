using System;
using System.Reflection;
using Harmony;

namespace Common.Harmony.PatchValidation
{
    internal interface IPatchValidation
    {
        ValidationResult IsValid(HarmonyInstance harmony, MethodInfo patchedMethod, Func<string, string> modLookup);
    }
}