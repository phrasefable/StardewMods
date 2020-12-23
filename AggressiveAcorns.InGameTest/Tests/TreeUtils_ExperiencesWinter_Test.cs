using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework;
using StardewValley;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Tests
{
    internal static class TreeUtils_ExperiencesWinter_Test
    {
        private static TestResult TheTest(string locationName, bool shouldExperienceWinter)
        {
            GameLocation location = Game1.getLocationFromName(locationName);
            if (location == null)
            {
                return new TestResult(TestOutcome.NotRun, $"Unable to find location with name '{locationName}'");
            }

            bool experiencesWinter = TreeUtils.ExperiencesWinter(location);

            return experiencesWinter == shouldExperienceWinter
                ? new TestResult(TestOutcome.Pass)
                : new TestResult(
                    TestOutcome.Fail,
                    $"Got {experiencesWinter}, expected {shouldExperienceWinter}"
                );
        }

        public static ITest BuildTest()
        {
            var cases = new CasedTest<string, bool>("ExperiencesWinter", TreeUtils_ExperiencesWinter_Test.TheTest);

            // Base Cases
            cases.AddCase("Farm", true);
            cases.AddCase("Greenhouse", false);
            cases.AddCase("Desert", false);

            // Farm Locations
            cases.AddCase("FarmHouse", false);
            cases.AddCase("FarmCave", false);
            cases.AddCase("Cellar", false);
            cases.AddCase("Cellar2", false);
            cases.AddCase("Cellar3", false);
            cases.AddCase("Cellar4", false);

            // Outdoors
            cases.AddCase("Town", true);
            cases.AddCase("Beach", true);
            cases.AddCase("Mountain", true);
            cases.AddCase("Forest", true);
            cases.AddCase("BusStop", true);
            cases.AddCase("Woods", true);
            cases.AddCase("Railroad", true);
            cases.AddCase("Backwoods", true);

            // Misc Indoors
            cases.AddCase("Tunnel", false);
            cases.AddCase("SkullCave", false);
            cases.AddCase("Mine", false);
            cases.AddCase("Sewer", false);

            // Buildings
            cases.AddCase("Blacksmith", false);
            cases.AddCase("ManorHouse", false);
            cases.AddCase("JoshHouse", false);
            cases.AddCase("HaleyHouse", false);
            cases.AddCase("SamHouse", false);
            cases.AddCase("SeedShop", false);
            cases.AddCase("Saloon", false);
            cases.AddCase("Trailer", false);
            cases.AddCase("Hospital", false);
            cases.AddCase("HarveyRoom", false);
            cases.AddCase("ElliottHouse", false);
            cases.AddCase("ScienceHouse", false);
            cases.AddCase("SebastianRoom", false);
            cases.AddCase("Tent", false);
            cases.AddCase("WizardHouse", false);
            cases.AddCase("AnimalShop", false);
            cases.AddCase("LeahHouse", false);
            cases.AddCase("SandyHouse", false);
            cases.AddCase("Club", false);
            cases.AddCase("ArchaeologyHouse", false);
            cases.AddCase("WizardHouseBasement", false);
            cases.AddCase("AdventureGuild", false);
            cases.AddCase("FishShop", false);
            cases.AddCase("BathHouse_Entry", false);
            cases.AddCase("BathHouse_MensLocker", false);
            cases.AddCase("BathHouse_WomensLocker", false);
            cases.AddCase("BathHouse_Pool", false);
            cases.AddCase("CommunityCenter", false);
            cases.AddCase("JojaMart", false);
            cases.AddCase("Trailer_Big", false);
            cases.AddCase("AbandonedJojaMart", false);
            cases.AddCase("MovieTheater", false);
            cases.AddCase("Sunroom", false);

            // Special Locations
            cases.AddCase("BeachNightMarket", true);
            cases.AddCase("Submarine", false);
            cases.AddCase("MermaidHouse", false);
            cases.AddCase("WitchSwamp", false);
            cases.AddCase("WitchHut", false);
            cases.AddCase("WitchWarpCave", false);
            cases.AddCase("BugLand", false);
            cases.AddCase("Summit", true);

            ITest test = cases.Guard_WorldReady();
            return test;
        }
    }
}
