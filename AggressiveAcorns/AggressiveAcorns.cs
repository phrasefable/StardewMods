using System.Collections.Generic;
using PhraseLib;
using StardewModdingAPI;

namespace AggressiveAcorns
{
    public class AggressiveAcorns : PatchingMod
    {
        private IModConfig _config;

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();
            Patch(helper);
        }

        protected override IEnumerable<IHarmonyPatch> LoadPatches()
        {
            yield return new Patch_Tree_DayUpdate(_config);
            if (_config.PreventScythe)
            {
                yield return new Patch_Tree_PerformToolAction();
            }

            if (_config.MaxPassibleGrowthStage != 0)
            {
                yield return new Patch_Tree_IsPassible(_config.MaxPassibleGrowthStage);
            }
        }
    }
}