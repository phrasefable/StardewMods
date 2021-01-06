using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Model;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using StardewValley;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Tests
{
    internal class TreeUtils_ExperiencesWinter_Test
    {
        private readonly ITestDefinitionFactory _factory;

        public TreeUtils_ExperiencesWinter_Test(ITestDefinitionFactory factory)
        {
            this._factory = factory;
        }

        public ITraversable Build()
        {
            ITestFixtureBuilder builder = _factory.CreateFixtureBuilder();
            builder.Key = "tree_utils_experiences_winter";
            builder.AddCondition(this._factory.Conditions.WorldReady);
            builder.AddChild(this.BuildTest_ExperiencesWinter());

            return builder.Build();
        }


        private ITraversable BuildTest_ExperiencesWinter()
        {
            ICasedTestBuilder<StringToBool> builder = _factory.CreateCasedTestBuilder<StringToBool>();
            builder.Key = "location_experiences_winter";
            builder.TestMethod = this.Test_ExperiencesWinter;
            builder.KeyGenerator = @case => @case.String.ToLower();

            // Base Cases
            builder.AddCases(
                new StringToBool("Farm", true),
                new StringToBool("Greenhouse", false),
                new StringToBool("Desert", false)
            );

            // Farm Locations
            builder.AddCases(
                new StringToBool("FarmHouse", false),
                new StringToBool("FarmCave", false),
                new StringToBool("Cellar", false),
                new StringToBool("Cellar2", false),
                new StringToBool("Cellar3", false),
                new StringToBool("Cellar4", false)
            );

            // Outdoors
            builder.AddCases(
                new StringToBool("Town", true),
                new StringToBool("Beach", true),
                new StringToBool("Mountain", true),
                new StringToBool("Forest", true),
                new StringToBool("BusStop", true),
                new StringToBool("Woods", true),
                new StringToBool("Railroad", true),
                new StringToBool("Backwoods", true)
            );

            // Misc Indoors
            builder.AddCases(
                new StringToBool("Tunnel", false),
                new StringToBool("SkullCave", false),
                new StringToBool("Mine", false),
                new StringToBool("Sewer", false)
            );

            // Buildings
            builder.AddCases(
                new StringToBool("Blacksmith", false),
                new StringToBool("ManorHouse", false),
                new StringToBool("JoshHouse", false),
                new StringToBool("HaleyHouse", false),
                new StringToBool("SamHouse", false),
                new StringToBool("SeedShop", false),
                new StringToBool("Saloon", false),
                new StringToBool("Trailer", false),
                new StringToBool("Hospital", false),
                new StringToBool("HarveyRoom", false),
                new StringToBool("ElliottHouse", false),
                new StringToBool("ScienceHouse", false),
                new StringToBool("SebastianRoom", false),
                new StringToBool("Tent", false),
                new StringToBool("WizardHouse", false),
                new StringToBool("AnimalShop", false),
                new StringToBool("LeahHouse", false),
                new StringToBool("SandyHouse", false),
                new StringToBool("Club", false),
                new StringToBool("ArchaeologyHouse", false),
                new StringToBool("WizardHouseBasement", false),
                new StringToBool("AdventureGuild", false),
                new StringToBool("FishShop", false),
                new StringToBool("BathHouse_Entry", false),
                new StringToBool("BathHouse_MensLocker", false),
                new StringToBool("BathHouse_WomensLocker", false),
                new StringToBool("BathHouse_Pool", false),
                new StringToBool("CommunityCenter", false),
                new StringToBool("JojaMart", false),
                new StringToBool("Trailer_Big", false),
                new StringToBool("AbandonedJojaMart", false),
                new StringToBool("MovieTheater", false),
                new StringToBool("Sunroom", false)
            );

            // Special Locations
            builder.AddCases(
                new StringToBool("BeachNightMarket", true),
                new StringToBool("Submarine", false),
                new StringToBool("MermaidHouse", false),
                new StringToBool("WitchSwamp", false),
                new StringToBool("WitchHut", false),
                new StringToBool("WitchWarpCave", false),
                new StringToBool("BugLand", false),
                new StringToBool("Summit", true)
            );

            return builder.Build();
        }


        private ITestResult Test_ExperiencesWinter(StringToBool @params)
        {
            string locationName = @params.String;
            bool shouldExperienceWinter = @params.Bool;

            GameLocation location = Game1.getLocationFromName(locationName);
            if (location == null)
            {
                return this._factory.BuildTestResult(Status.Error, $"Unable to find location with name '{locationName}'");
            }

            bool experiencesWinter = TreeUtils.ExperiencesWinter(location);

            return experiencesWinter == shouldExperienceWinter
                ? this._factory.BuildTestResult(Status.Pass)
                : this._factory.BuildTestResult(Status.Fail,
                    $"Got {experiencesWinter}, expected {shouldExperienceWinter}.");
        }
    }
}
