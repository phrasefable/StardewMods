using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Builders;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model;
using StardewValley;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Tests
{
    internal class TreeUtils_ExperiencesWinter_Test : ITestFixtureDefinition
    {
        public ITestFixture GetFixture(IBuilderFactory factory)
        {
            var fixtureBuilder = factory.CreateFixtureBuilder();
            fixtureBuilder.SetKey("tree_utils_experiences_winter");
            fixtureBuilder.AddCondition(Utils.Condition_WorldReady);
            fixtureBuilder.AddTest(this.GetTestBuilder_ExperiencesWinter(factory));

            return fixtureBuilder.Build();
        }


        private IBuilder<IBaseTest> GetTestBuilder_ExperiencesWinter(IBuilderFactory factory)
        {
            var testBuilder = factory.CreateCasedTestBuilder<StringToBool>();
            testBuilder.SetKey("location_experiences_winter");
            testBuilder.SetTestMethod(this.TestLocationExperiencesWinter);

            // Base Cases
            testBuilder.AddCases(
                new StringToBool("Farm", true),
                new StringToBool("Greenhouse", false),
                new StringToBool("Desert", false)
            );

            // Farm Locations
            testBuilder.AddCases(
                new StringToBool("FarmHouse", false),
                new StringToBool("FarmCave", false),
                new StringToBool("Cellar", false),
                new StringToBool("Cellar2", false),
                new StringToBool("Cellar3", false),
                new StringToBool("Cellar4", false)
            );

            // Outdoors
            testBuilder.AddCases(
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
            testBuilder.AddCases(
                new StringToBool("Tunnel", false),
                new StringToBool("SkullCave", false),
                new StringToBool("Mine", false),
                new StringToBool("Sewer", false)
            );

            // Buildings
            testBuilder.AddCases(
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
            testBuilder.AddCases(
                new StringToBool("BeachNightMarket", true),
                new StringToBool("Submarine", false),
                new StringToBool("MermaidHouse", false),
                new StringToBool("WitchSwamp", false),
                new StringToBool("WitchHut", false),
                new StringToBool("WitchWarpCave", false),
                new StringToBool("BugLand", false),
                new StringToBool("Summit", true)
            );

            return testBuilder;
        }


        private IResult TestLocationExperiencesWinter(StringToBool @params)
        {
            string locationName = @params.String;
            bool shouldExperienceWinter = @params.Bool;

            GameLocation location = Game1.getLocationFromName(locationName);
            if (location == null)
            {
                return new Result(Status.Error, $"Unable to find location with name '{locationName}'");
            }

            bool experiencesWinter = TreeUtils.ExperiencesWinter(location);

            return experiencesWinter == shouldExperienceWinter
                ? new Result(Status.Pass)
                : new Result(
                    Status.Fail,
                    $"Got {experiencesWinter}, expected {shouldExperienceWinter}"
                );
        }
    }
}