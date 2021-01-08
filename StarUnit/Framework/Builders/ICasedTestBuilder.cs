using System;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Framework.Builders
{
    // TODO: Cased tests behave unexpectedly as their children are not direct children of the containing suite
    // TODO:   -> leading to unexpected behaviour regarding container's BeforeEach.
    // TODO:   Either make before each cascade to all descendant leaves (bad), or somehow inject cases upwards for
    // TODO:   test running only, but keeping results separate. Could do this by adding bool flag to suites (with
    // TODO:   validation in builder) (easier), or reinstating test grouping and traversable branch types (harder).
    public interface ICasedTestBuilder<TCaseParams> : IBuilder<ITraversableGrouping>, ITraversableBuilder
    {
        public Func<TCaseParams, ITestResult> TestMethod { set; }

        public void AddCases(params TCaseParams[] cases);

        public Func<TCaseParams, string> KeyGenerator { set; }
        public Func<TCaseParams, string> LongNameGenerator { set; }
    }
}