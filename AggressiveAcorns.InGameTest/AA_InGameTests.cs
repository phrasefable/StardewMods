using Microsoft.Xna.Framework;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Tests;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Utilities;
using Phrasefable.StardewMods.StarUnit;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest
{
    public class AA_InGameTests : Mod
    {
        private ITestDefinitionFactory _factory;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += this.OnGameLoopOnGameLaunched;
            helper.Events.Content.AssetRequested += TestTree.Content_AssetRequested;
            TestTree.InvalidateData = () => this.Helper.GameContent.InvalidateCache(AggressiveAcorns.Path_WildTreeData);
            this.SetUpCommands(helper);
        }

        private void OnGameLoopOnGameLaunched(object sender, GameLaunchedEventArgs args)
        {
            IStarUnitApi starUnitApi = this.Helper.ModRegistry.GetApi<IStarUnitApi>("Phrasefable.StarUnit");
            this._factory = starUnitApi.TestDefinitionFactory;

            starUnitApi.Register("aa", this.GetTestNodes().ToArray());
        }

        private IEnumerable<ITraversable> GetTestNodes()
        {
            yield return new PassableTests(this._factory).Build();
            yield return new ToolActionTests(this._factory).Build();
        }

        private void SetUpCommands(IModHelper helper)
        {
            helper.ConsoleCommands.Add(
                "aa_update_all",
                "Calls DayUpdate on all trees in current location",
                (name, args) =>
                {
                    if (args.Length > 1)
                    {
                        this.Monitor.Log($"Invalid arguments '{args}'");
                        return;
                    }

                    int reps = 1;
                    if (args.Length == 1)
                    {
                        bool isInt = int.TryParse(args[0], out reps);
                        if (!isInt)
                        {
                            this.Monitor.Log($"Not an int '{args}'");
                            return;
                        }
                    }

                    GameLocation location = Game1.player.currentLocation;
                    for (int i = 0; i < reps; i++)
                    {
                        IEnumerable<Tree> trees = location.terrainFeatures.Values
                            .Where(feature => feature is Tree)
                            .Cast<Tree>()
                            .ToList();

                        foreach (Tree tree in trees)
                        {
                            tree.dayUpdate();
                        }
                    }
                }
            );

            helper.ConsoleCommands.Add(
                "aa_tester",
                "Set up testing scenario.",
                (name, args) =>
                {
                    GameLocation farm = Game1.getLocationFromName("Farm");
                    if (farm == null)
                    {
                        this.Monitor.Log("Could not find farm", LogLevel.Error);
                        return;
                    }
                    Game1.player.warpFarmer(LocationUtils.WarpFarm);
                    farm.ClearLocation();
                    

                    Game1.player.clearBackpack();
                    Game1.player.addItemToInventory(ItemRegistry.Create("(W)0"));
                    Game1.player.addItemToInventory(ItemRegistry.Create("(W)66"));
                    Game1.player.addItemToInventory(ItemRegistry.Create("(T)IridiumAxe"));
                    Game1.player.addItemToInventory(ItemRegistry.Create("(T)IridiumPickaxe"));
                    Game1.player.addItemToInventory(ItemRegistry.Create("(T)IridiumHoe"));
                    Game1.player.addItemToInventory(ItemRegistry.Create("(O)309", 99));


                    Tree lastTree = null;
                    foreach (int stage in TreeUtils.Stages)
                    {
                        if (lastTree == null)
                        {
                            lastTree = TreeUtils.GetFarmTreeLonely(stage);
                        }
                        else
                        {
                            Vector2 newTile = new Vector2(lastTree.Tile.X + 2, lastTree.Tile.Y);
                            lastTree = TreeUtils.PlantTree(lastTree.Location, newTile, lastTree.treeType.Value, stage);

                        }
                    }

                    TreeUtils.PlantTree(LocationUtils.WarpFarm.GetLocation(), LocationUtils.WarpFarm.GetTargetTile() + new Vector2(-4, -2), TestTree.Id, Tree.treeStage);
                }
            );
        }
    }
}