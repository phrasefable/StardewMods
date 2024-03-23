using System;
using System.Collections.Generic;
using System.Linq;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using Phrasefable.StardewMods.StarUnit.Internal.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Internal
{
    internal class TestRegistry
    {
        private readonly Action<string> _writer;
        private readonly Action<string> _errorWriter;

        private readonly IDictionary<string, TraversableGrouping> _testRoots;

        public IEnumerable<ITraversable> TestRoots => this._testRoots.Values;


        public TestRegistry(Action<string> writer, Action<string> errorWriter)
        {
            this._testRoots = new Dictionary<string, TraversableGrouping>();

            this._writer = writer;
            this._errorWriter = errorWriter;
        }


        public void Register(string modId, params ITraversable[] testNodes)
        {
            TraversableGrouping root = this.GetRoot(modId);

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


        private TraversableGrouping GetRoot(string modId)
        {
            if (this._testRoots.TryGetValue(modId, out TraversableGrouping root))
            {
                return root;
            }

            root = new TraversableGrouping { Key = modId, Conditions = Array.Empty<Func<IResult>>() };
            this._testRoots[modId] = root;
            return root;
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
    }
}