using System.Collections.Generic;
using Common.Harmony;
using Harmony;
using JetBrains.Annotations;
using StardewModdingAPI;
using static Common.Harmony.PatchValidation.Utilities;

namespace AggressiveAcorns
{
    [UsedImplicitly]
    public class AggressiveAcorns : Mod
    {
        private IModConfig _config;

        private HarmonyInstance _harmony;
        private readonly List<IHarmonyPatchInfo> _patches = new List<IHarmonyPatchInfo>();


        public override void Entry([NotNull] IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();
            _harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            SetupPatches(helper);

            helper.Events.GameLoop.GameLaunched += (s, args) => ValidatePatches(
                _harmony,
                _patches,
                Monitor,
                helper.ModRegistry
            );
        }


        private void SetupPatches(IModHelper helper)
        {
            _patches.Add(Patch_Tree_DayUpdate.Initialize(Monitor, _config, helper.Reflection));

            if (!_config.DoScythesDestroySeedlings)
            {
                _patches.Add(Patch_Tree_PerformToolAction.Initialize(Monitor));
            }

            if (_config.MaxPassibleGrowthStage != new ModConfig().MaxPassibleGrowthStage)
            {
                _patches.Add(Patch_Tree_IsPassible.Initialize(Monitor, _config));
            }

            _patches.ForEach(p => p.ApplyPatch(_harmony));
        }
    }
}
