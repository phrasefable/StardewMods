using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using StardewValley;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Tests
{
    internal class TreeUtilsTests
    {
        private readonly ITestDefinitionFactory _factory;

        public TreeUtilsTests(ITestDefinitionFactory factory)
        {
            this._factory = factory;
        }

        public ITraversable Build()
        {
            ITestFixtureBuilder builder = _factory.CreateFixtureBuilder();
            builder.Key = "tree_utils_methods";
            builder.AddCondition(this._factory.Conditions.WorldReady);
            builder.AddChild(this.BuildTest_ExperiencesWinter());

            return builder.Build();
        }


        private ITraversable BuildTest_ExperiencesWinter()
        {
            ICasedTestBuilder<(string LocationName, bool ShouldExperienceWinter)> builder =
                _factory.CreateCasedTestBuilder<(string, bool)>();
            builder.Key = "location_experiences_winter";
            builder.TestMethod = this.Test_ExperiencesWinter;
            builder.KeyGenerator = @case => @case.LocationName.ToLower();

            // Base Cases
            builder.AddCases(
                ("Farm", true),
                ("Greenhouse", false),
                ("Desert", false)
            );

            // Farm Locations
            builder.AddCases(
                ("FarmHouse", false),
                ("FarmCave", false),
                ("Cellar", false),
                ("Cellar2", false),
                ("Cellar3", false),
                ("Cellar4", false)
            );

            // Outdoors
            builder.AddCases(
                ("Town", true),
                ("Beach", true),
                ("Mountain", true),
                ("Forest", true),
                ("BusStop", true),
                ("Woods", true),
                ("Railroad", true),
                ("Backwoods", true)
            );

            // Misc Indoors
            builder.AddCases(
                ("Tunnel", false),
                ("SkullCave", false),
                ("Mine", false),
                ("Sewer", false)
            );

            // Buildings
            builder.AddCases(
                ("Blacksmith", false),
                ("ManorHouse", false),
                ("JoshHouse", false),
                ("HaleyHouse", false),
                ("SamHouse", false),
                ("SeedShop", false),
                ("Saloon", false),
                ("Trailer", false),
                ("Hospital", false),
                ("HarveyRoom", false),
                ("ElliottHouse", false),
                ("ScienceHouse", false),
                ("SebastianRoom", false),
                ("Tent", false),
                ("WizardHouse", false),
                ("AnimalShop", false),
                ("LeahHouse", false),
                ("SandyHouse", false),
                ("Club", false),
                ("ArchaeologyHouse", false),
                ("WizardHouseBasement", false),
                ("AdventureGuild", false),
                ("FishShop", false),
                ("BathHouse_Entry", false),
                ("BathHouse_MensLocker", false),
                ("BathHouse_WomensLocker", false),
                ("BathHouse_Pool", false),
                ("CommunityCenter", false),
                ("JojaMart", false),
                ("Trailer_Big", false),
                ("AbandonedJojaMart", false),
                ("MovieTheater", false),
                ("Sunroom", false)
            );

            // Special Locations
            builder.AddCases(
                ("BeachNightMarket", true),
                ("Submarine", false),
                ("MermaidHouse", false),
                ("WitchSwamp", false),
                ("WitchHut", false),
                ("WitchWarpCave", false),
                ("BugLand", false),
                ("Summit", true)
            );

            return builder.Build();
        }


        private ITestResult Test_ExperiencesWinter((string LocationName, bool ShouldExperienceWinter) @params)
        {
            (string locationName, bool shouldExperienceWinter) = @params;

            GameLocation location = Game1.getLocationFromName(locationName);
            if (location == null)
            {
                return this._factory.BuildTestResult(
                    Status.Error,
                    $"Unable to find location with name '{locationName}'"
                );
            }

            bool experiencesWinter = TreeUtils.ExperiencesWinter(location);

            return experiencesWinter == shouldExperienceWinter
                ? this._factory.BuildTestResult(Status.Pass)
                : this._factory.BuildTestResult(
                    Status.Fail,
                    $"Got {experiencesWinter}, expected {shouldExperienceWinter}."
                );
        }
    }
}
