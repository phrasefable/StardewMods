using StardewModdingAPI.Events;
using StardewValley.GameData.WildTrees;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest
{
    internal class TestTree
    {

        public static readonly string Id = "AA.IGT_TestTree";
        public static Action InvalidateData;

        // Note can directly change treeData.whatever and the tree will behave accordingly in the game.
        private static WildTreeData _treeData = null;
        public static WildTreeData TreeData
        {
            get
            {
                if (_treeData == null)
                {
                    ResetData();
                }
                return _treeData;
            }
        }

        public static void ResetData()
        {
            _treeData = new WildTreeData
            {
                Textures = [new WildTreeTextureData() { Texture = "TerrainFeatures/mystic_tree" }],
                GrowthChance = 0,
                FertilizedGrowthChance = 0,
            };
            InvalidateData();
        }

        public static void Content_AssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (!e.NameWithoutLocale.IsEquivalentTo(AggressiveAcorns.Path_WildTreeData)) return;

            e.Edit(
                asset =>
                {
                    var editor = asset.AsDictionary<string, WildTreeData>();
                    editor.Data[Id] = TreeData;
                },
                AssetEditPriority.Early
            );

        }
    }
}
