using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;

namespace Common.Harmony.PatchValidation
{
    internal class ExclusivePatch : IPatchValidation
    {
        public static readonly IPatchValidation Instance = new ExclusivePatch();


        private ExclusivePatch() { }


        public ValidationResult IsValid(
            HarmonyInstance harmony,
            MethodInfo patchedMethod,
            Func<string, string> modLookup)
        {
            Patches info = harmony.GetPatchInfo(patchedMethod);
            List<string> conflicts = info.Owners.Where(id => id != harmony.Id).ToList();

            var result = new ValidationResult {IsValid = true};

            if (!conflicts.Any()) return result;

            string qualifiedName = $"{patchedMethod.DeclaringType.FullName}.{patchedMethod.Name}";

            result.IsValid = false;
            result.Information.Add(
                $"Method {qualifiedName} is patched by other mods:"
            );
            conflicts.ForEach(mod => result.Information.Add($" - {modLookup(mod)}"));
            result.Information.Add(
                $"{modLookup("this")} expects that it should be the only mod patching {qualifiedName}."
            );
            result.Information.Add(
                $"As such {modLookup("this")} may experience errors, or may cause errors in the conflicting mods."
            );

            int idx = result.Information.Count - 1;
            result.Information[idx] = result.Information[idx] + "\n";

            return result;
        }
    }
}