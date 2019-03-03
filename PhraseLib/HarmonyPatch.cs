using System;
using System.Linq;
using System.Reflection;
using Harmony;

namespace PhraseLib
{
    public interface IHarmonyPatch
    {
        void ApplyPatch(HarmonyInstance harmony);

        bool IsValid(HarmonyInstance harmony, out string errors);
    }


    public abstract class HarmonyPatch : IHarmonyPatch
    {
        public abstract void ApplyPatch(HarmonyInstance harmony);

        /// <summary>
        /// Checks if the patching is valid.
        /// </summary>
        /// <param name="harmony"></param>
        /// <param name="errors">String to be printed in console listing errors</param>
        /// <returns></returns>
        public abstract bool IsValid(HarmonyInstance harmony, out string errors);

        protected bool IsExclusivePatch(HarmonyInstance harmony, out string overlaps)
        {
            var info = harmony.GetPatchInfo(GetTargetMethod());
            var conflicts = info.Owners.Where(id => id != harmony.Id).ToList();

            if (conflicts.Any())
            {
                overlaps = $"Patch of method {TargetName} in {GetType().Name} is not exclusive. " +
                           $"Method also patched in: {string.Join(", ", conflicts)}";
                return false;
            }

            overlaps = null;
            return true;
        }

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

        protected MethodInfo GetTargetMethod()
        {
            return TargetType.GetMethod(TargetName, TargetParameters);
        }
    }
}