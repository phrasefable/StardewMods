using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Loggers;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework
{
    internal class TestSuite : ITest
    {
        public string Name { get; }

        [CanBeNull] public Action BeforeAll { private get; set; }
        [CanBeNull] public Action BeforeEach { private get; set; }
        [CanBeNull] public Action AfterEach { private get; set; }
        [CanBeNull] public Action AfterAll { private get; set; }

        private readonly List<ITest> _tests = new List<ITest>();


        public void Add(ITest test)
        {
            this._tests.Add(test);
        }

        public void RunTest()
        {
            BeforeAll?.Invoke();
            foreach (ITest test in this._tests)
            {
                BeforeEach?.Invoke();
                test.RunTest();
                AfterEach?.Invoke();
            }

            AfterAll?.Invoke();
        }


        public ILogger GetResults()
        {
            var logger = new AggregateLogger();
            this._tests.ForEach(test => logger.Add(test.GetResults()));
            return logger;
        }
    }
}