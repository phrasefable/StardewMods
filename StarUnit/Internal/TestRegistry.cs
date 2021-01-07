using System;
using System.Collections.Generic;
using System.Linq;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Internal.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Internal
{
    internal class TestRegistry
    {
        private readonly IDictionary<string, TestRoot> _testRoots = new Dictionary<string, TestRoot>();

        private readonly Action<string> _writer;
        private readonly Action<string> _errorWriter;

        public IEnumerable<ITestSuite> TestRoots => this._testRoots.Values.Select(root => root.TestSuite);

        public TestRegistry(Action<string> writer, Action<string> errorWriter)
        {
            this._writer = writer;
            this._errorWriter = errorWriter;
        }

        private static IEnumerable<string> FindDuplicates(IEnumerable<string> list1, IEnumerable<string> list2)
        {
            ICollection<string> uniqueStrings = new HashSet<string>(list1);
            foreach (string s in list2)
            {
                if (uniqueStrings.Contains(s))
                {
                    yield return s;
                }

                uniqueStrings.Add(s);
            }
        }


        public void Register(string modId, params ITraversable[] testNodes)
        {
            if (!this._testRoots.TryGetValue(modId, out TestRoot root))
            {
                root = new TestRoot(modId);
                this._testRoots[modId] = root;
            }

            IEnumerable<string> duplicates = TestRegistry.FindDuplicates(
                    root.Children.Select(c => c.Key),
                    testNodes.Select(c => c.Key)
                )
                .ToArray();

            if (duplicates.Any())
            {
                this._errorWriter(
                    $"Failed to register {testNodes.Length} items at '{modId}' due to {duplicates.Count()} duplicates:"
                );
                this._errorWriter("    " + string.Join(" ", duplicates));
                return;
            }

            foreach (ITraversable node in testNodes)
            {
                root.Children.Add(node);
            }

            this._writer($"Registered {testNodes.Length} test node(s) to root '{modId}'.");
        }

        // ============================================================================================================

        private class TestRoot
        {
            private readonly TestSuite _testSuite;
            public ITestSuite TestSuite => this._testSuite;
            public ICollection<ITraversable> Children => this._testSuite.Children;

            public TestRoot(string key)
            {
                this._testSuite = new TestSuite {Key = key};
            }
        }
    }
}