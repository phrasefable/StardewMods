using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Harmony;
using JetBrains.Annotations;

namespace PhraseLib {

    public interface IHarmonyPatch {

        void ApplyPatch(HarmonyInstance harmony);

        bool IsValid(HarmonyInstance harmony, out string errors);
    }


    internal abstract class HarmonyPatch : IHarmonyPatch {


        /// <summary>
        /// The type of the patch targets. Best to use <code>typeof(TheType)</code>
        /// </summary>
        protected abstract Type TargetType { get; }

        /// <summary>
        /// The method that the patch targets. Best to use <code>nameof(TheType.TheMethod)</code>
        /// </summary>
        protected abstract string TargetName { get; }

        /// <summary>
        /// The types of the parameters of the method being patched.
        /// </summary>
        protected abstract Type[] TargetParameters { get; }

        [CanBeNull] protected abstract MethodInfo PrefixPatch { get; }

        [CanBeNull] protected abstract MethodInfo TranspilerPatch { get; }

        [CanBeNull] protected abstract MethodInfo PostfixPatch { get; }


        /// <summary>
        /// Checks if the patching is valid.
        /// </summary>
        /// <param name="harmony"></param>
        /// <param name="errors">String to be printed in console listing errors. Null if no errors</param>
        /// <returns></returns>
        public abstract bool IsValid(HarmonyInstance harmony, out string errors);


        protected MethodInfo GetTargetMethod() {
            return TargetType.GetMethod(TargetName, TargetParameters);
        }


        public void ApplyPatch(HarmonyInstance harmony) { }


        protected bool IsExclusivePatch(HarmonyInstance harmony, out string overlaps) {
            var info = harmony.GetPatchInfo(GetTargetMethod());
            var conflicts = info.Owners.Where(id => id != harmony.Id).ToList();

            if (conflicts.Any()) {
                var builder = new StringBuilder();
                builder.Append($"The method {TargetName} of {GetType().Name} is also patched by the following mods:");
                foreach (var conflict in conflicts) {
                    builder.AppendLine("    - " + conflict);
                }

                builder.AppendLine(
                    "    These mods may conflict. If you have issues, check whether they duplicate functionality, or if they can be configured to coexist.");
                overlaps = builder.ToString();
                return false;
            }

            overlaps = null;
            return true;
        }
    }


    // public abstract class PrefixPatch : HarmonyPatch {
    //     private const BindingFlags PatchBindingFlags = BindingFlags.NonPublic | BindingFlags.Static;
    //
    //
    //
    //
    //     public sealed override void ApplyPatch(HarmonyInstance harmony) {
    //         harmony.Patch(GetTargetMethod(), GetPatch());
    //     }
    //
    //
    //     private HarmonyMethod GetPatch() {
    //         return new HarmonyMethod(GetType().GetMethod(PatchMethod, PatchBindingFlags));
    //     }
    // }

}