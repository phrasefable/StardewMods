using System;
using System.Linq;
using System.Reflection;
using Harmony;

namespace AggressiveAcorns
{
    
    public interface IHarmonyPatch
    {
        void ApplyPatch(HarmonyInstance harmony);
        bool IsValid(HarmonyInstance harmony, out string errors);
    }
    
    
    public abstract class HarmonyPatch : IHarmonyPatch
    {
        public abstract void ApplyPatch(HarmonyInstance harmony);

        public abstract bool IsValid(HarmonyInstance harmony, out string errors);

        protected bool IsExclusivePatch(HarmonyInstance harmony, out string overlaps)
        {
            var info = harmony.GetPatchInfo(GetTargetMethod());
            var conflicts = info.Owners.Where(id => id != harmony.Id).ToList();

            if (conflicts.Any())
            {
                overlaps = $"Patch of method {TargetName} in {this.GetType().Name} is not exclusive. " +
                           $"Method also patched in: {string.Join(", ", conflicts)}";
                return false;
            }

            overlaps = null;
            return true;
        }
        
        protected abstract Type TargetType { get; }
        
        protected abstract string TargetName { get; }
        
        protected abstract Type[] TargetParameters { get; }
        
        protected MethodInfo GetTargetMethod()
        {
            return TargetType.GetMethod(TargetName, TargetParameters);
        }
    }
    

    public abstract class PrefixPatch : HarmonyPatch
    {

        private const BindingFlags PatchBindingFlags = BindingFlags.NonPublic | BindingFlags.Static;

        protected abstract string PatchMethod { get; }

        public sealed override void ApplyPatch(HarmonyInstance harmony)
        {
            harmony.Patch(GetTargetMethod(), GetPatch());
        }
        
        private HarmonyMethod GetPatch()
        {
            return new HarmonyMethod(GetType().GetMethod(PatchMethod, PatchBindingFlags));
        }
    }
}