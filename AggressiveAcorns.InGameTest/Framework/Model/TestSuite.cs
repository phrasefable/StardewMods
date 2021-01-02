using System;
using System.Collections.Generic;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model
{
    public class TestSuite : ITestSuite
    {
        public string Key { get; set; }
        public string LongName { get; set; }

        public IEnumerable<Func<IResult>> Conditions { get; set; }

        public Action BeforeAll { get; set; }
        public Action BeforeEach { get; set; }
        public Action AfterEach { get; set; }
        public Action AfterAll { get; set; }

        public IEnumerable<TestNode> Children { get; set; }
    }
}
