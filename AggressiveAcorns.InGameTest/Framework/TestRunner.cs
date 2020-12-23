using System.Linq;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework
{
    public class TestRunner
    {
        public void RunTest(ITestNode testNode)
        {
            bool mayRun = testNode.Conditions.All(
                condition => condition.Invoke().status == ResultStatus.Pass
            );
        }
    }
}
