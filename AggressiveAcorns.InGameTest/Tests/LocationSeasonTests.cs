using Phrasefable.StardewMods.AggressiveAcorns.Framework;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Utilities;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using StardewValley;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Tests
{
    internal class LocationSeasonTests
    {
        private readonly ITestDefinitionFactory _factory;

        public LocationSeasonTests(ITestDefinitionFactory factory)
        {
            this._factory = factory;
        }


        public ITraversable Build()
        {
            ITestFixtureBuilder builder = _factory.CreateFixtureBuilder();
            builder.Key = "seasons";
            builder.AddCondition(this._factory.Conditions.WorldReady);

            string initSeason = null;
            builder.BeforeAll = () => initSeason = Game1.currentSeason;
            builder.AfterAll = () => SeasonUtils.SetSeason(initSeason);

            builder.AddChild(this.BuildTest_ExperiencesWinter());
            builder.AddChild(this.BuildTest_ExperiencingWinter());

            return builder.Build();
        }


        private ITraversable BuildTest_ExperiencesWinter()
        {
            ICasedTestBuilder<(string LocationName, bool ShouldExperienceWinter)> builder =
                _factory.CreateCasedTestBuilder<(string, bool)>();
            builder.Key = "experiences_winter";
            builder.TestMethod = this.Test_ExperiencesWinter;
            builder.KeyGenerator = @case => @case.LocationName.ToLower();
            builder.AddCases(LocationSeasonTests.LocationDefs);

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

            Season.Spring.SetSeason();
            bool experiencesWinter = location.ExperiencesWinter();

            return experiencesWinter == shouldExperienceWinter
                ? this._factory.BuildTestResult(Status.Pass)
                : this._factory.BuildTestResult(
                    Status.Fail,
                    $"Got {experiencesWinter}, expected {shouldExperienceWinter}."
                );
        }


        private ITraversable BuildTest_ExperiencingWinter()
        {
            ICasedTestBuilder<(string LocationName, bool ShouldExperienceWinter)> builder =
                _factory.CreateCasedTestBuilder<(string, bool)>();
            builder.Key = "experiencing_winter";
            builder.TestMethod = this.Test_ExperiencingWinter;
            builder.KeyGenerator = @case => @case.LocationName.ToLower();
            builder.AddCases(LocationSeasonTests.LocationDefs);

            return builder.Build();
        }


        private ITestResult Test_ExperiencingWinter((string LocationName, bool ShouldExperienceWinter) @params)
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

            Season.Winter.SetSeason();
            bool experiencesWinter = location.ExperiencingWinter();

            return experiencesWinter == shouldExperienceWinter
                ? this._factory.BuildTestResult(Status.Pass)
                : this._factory.BuildTestResult(
                    Status.Fail,
                    $"Got {experiencesWinter}, expected {shouldExperienceWinter}."
                );
        }


        private static readonly (string LocationName, bool ShouldExperienceWinter)[] LocationDefs = new[]
        {
            // Base cases
            ("Farm", true),
            ("Greenhouse", false),
            ("Desert", false),

            // Farm interiors
            ("FarmHouse", false),
            ("FarmCave", false),
            ("Cellar", false),
            // ("Cellar2", false), // TODO - removed in 1.5??
            // ("Cellar3", false),
            // ("Cellar4", false),

            // Outdoors
            ("Town", true),
            ("Beach", true),
            ("Mountain", true),
            ("Forest", true),
            ("BusStop", true),
            ("Woods", true),
            ("Railroad", true),
            ("Backwoods", true),

            // Misc indoors
            ("Tunnel", false),
            ("SkullCave", false),
            ("Mine", false),
            ("Sewer", false),

            // Buildings
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
            ("Sunroom", false),

            // Special locations
            ("BeachNightMarket", true),
            ("Submarine", false),
            ("MermaidHouse", false),
            ("WitchSwamp", false),
            ("WitchHut", false),
            ("WitchWarpCave", false),
            ("BugLand", true), // !location.IsOutdoors && location.treatAsOutdoors
            ("Summit", true),

            // 1.5
            ("IslandSouth", false),
            ("IslandSouthEast", false),
            ("IslandSouthEastCave", false),
            ("IslandEast", false),
            ("IslandWest", false),
            ("IslandNorth", false),
            ("IslandHut", false),
            ("IslandWestCave1", false),
            ("IslandNorthCave1", false),
            ("IslandFieldOffice", false),
            ("IslandFarmHouse", false),
            ("CaptainRoom", false),
            ("IslandShrine", false),
            ("IslandFarmCave", false),
            ("Caldera", false),
            ("LeoTreeHouse", false),
            ("QiNutRoom", false)
        };
    }
}
