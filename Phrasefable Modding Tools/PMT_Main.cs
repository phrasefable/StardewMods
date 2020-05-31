using JetBrains.Annotations;
using StardewModdingAPI;

namespace Phrasefable.StardewMods.ModdingTools
{
    [UsedImplicitly]
    public partial class PhrasefableModdingTools : Mod
    {
        public override void Entry([NotNull] IModHelper helper)
        {
            SetUp_Ground();

            SetUp_Tally();

            SetUp_EventLogging();

            SetUp_Clear();
        }
    }
}
