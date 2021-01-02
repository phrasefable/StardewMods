using System;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Builders
{
    public interface ITestFixtureBuilder :
        IBuilder<ITestSuite>,
        IIdentifiableBuilder,
        IConditionalBuilder
    {
        public void SetBeforeAllAction(Action value);
        public void SetBeforeEachAction(Action value);
        public void SetAfterEachAction(Action value);
        public void SetAfterAllAction(Action value);

        public void AddChild(ITestSuite child);
        public void AddChild(ITest child);
    }
}