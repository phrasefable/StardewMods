using System;
using System.Reflection;
using Common.Harmony.PatchTypes;
using Common.Harmony.PatchValidation;
using Harmony;

namespace Common.Harmony
{
    internal class HarmonyPatchInfo : IHarmonyPatchInfo
    {
        public MethodInfo PatchTarget { get; }
        public MethodInfo PatchSource { get; }

        private readonly IPatchApplicator _applicator;
        private readonly IPatchValidation _validationModel;


        public HarmonyPatchInfo(
            MethodInfo patchTarget,
            MethodInfo patchSource,
            IPatchApplicator patchType,
            IPatchValidation validationModel)
        {
            PatchTarget = patchTarget;
            PatchSource = patchSource;
            _applicator = patchType;
            _validationModel = validationModel;
        }


        public ValidationResult IsValid(HarmonyInstance harmony, Func<string, string> modLookup)
        {
            return _validationModel.IsValid(harmony, this.PatchTarget, modLookup);
        }


        public void ApplyPatch(HarmonyInstance harmony)
        {
            _applicator.ApplyPatch(harmony, this);
        }
    }
}