using System.Collections.Generic;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model
{
    public class TestResult : ITestResult
    {
        private readonly IList<ITestResult> _children = new List<ITestResult>();

        public string Key { get; set; }
        public string LongName { get; set; }
        public bool IsAggregate { get; set; }
        public IResult Result { get; set; }
        public IEnumerable<ITestResult> Children => this._children;

        public TestResult() { }

        public TestResult(IIdentifiable identifiable)
        {
            this.Key = identifiable.Key;
            this.LongName = identifiable.LongName;
        }

        public void AggregateResults(IEnumerable<ITestResult> results)
        {
            foreach (ITestResult result in results)
            {
                this._children.Add(result);
            }
        }
    }
}