using System;
using System.Collections.Generic;
using Harmony;
using JetBrains.Annotations;
using Phrasefable.StardewMods.Common.Harmony;
using StardewModdingAPI;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns
{
    [UsedImplicitly]
    public class AggressiveAcorns : Mod
    {
        internal static Action<string> ErrorLogger;
        internal static IConfigAdaptor Config;


        private HarmonyInstance _harmony;
        private ICollection<IHarmonyPatchInfo> _patches;


        public override void Entry([NotNull] IModHelper helper)
        {
            AggressiveAcorns.Config = new ConfigAdaptor(helper.ReadConfig<ModConfig>());
            AggressiveAcorns.ErrorLogger = message => this.Monitor.Log(message, LogLevel.Error);

            this.SetUpPatches();
        }


        private void SetUpPatches()
        {
            this._harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            this._patches = new List<IHarmonyPatchInfo>
            {
                this.BuildPatch_Tree_IsPassable(),
                this.BuildPatch_Tree_PerformToolAction(),
                this.BuildPatch_Tree_DayUpdate()
            };

            // TODO - reimplement the non-exclusive patch thing.

            foreach (IHarmonyPatchInfo patch in this._patches)
            {
                patch.Apply(this._harmony);
            }
        }


        private IHarmonyPatchInfo BuildPatch_Tree_IsPassable()
        {
            return new HarmonyPatchInfo(
                AccessTools.Method(typeof(StardewValley.TerrainFeatures.Tree), nameof(Tree.isPassable)),
                AccessTools.Method(typeof(TreePatches), nameof(TreePatches.IsPassable_Postfix)),
                PatchType.Postfix
            );
        }


        private IHarmonyPatchInfo BuildPatch_Tree_PerformToolAction()
        {
            return new HarmonyPatchInfo(
                AccessTools.Method(typeof(StardewValley.TerrainFeatures.Tree), nameof(Tree.performToolAction)),
                AccessTools.Method(typeof(TreePatches), nameof(TreePatches.PerformToolAction_Prefix)),
                PatchType.Prefix
            );
        }


        private IHarmonyPatchInfo BuildPatch_Tree_DayUpdate()
        {
            return new HarmonyPatchInfo(
                AccessTools.Method(typeof(StardewValley.TerrainFeatures.Tree), nameof(Tree.dayUpdate)),
                AccessTools.Method(typeof(TreePatches), nameof(TreePatches.DayUpdate_Prefix)),
                PatchType.Prefix
            );
        }
    }
}
