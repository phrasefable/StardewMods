using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Loggers;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework
{
    internal interface ITest
    {
        public string Name { get; }
        public void RunTest();
        public ILogger GetResults();
    }
}