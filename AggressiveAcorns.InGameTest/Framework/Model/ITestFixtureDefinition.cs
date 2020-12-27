using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Builders;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model
{
    public interface ITestFixtureDefinition
    {
        public ITestFixture GetFixture(IBuilderFactory factory);
    }
}