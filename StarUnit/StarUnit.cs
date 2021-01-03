using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Phrasefable.StardewMods.StarUnit.Framework.Model;
using Phrasefable.StardewMods.StarUnit.Internal;
using Phrasefable.StardewMods.StarUnit.Internal.Builders;
using StardewModdingAPI;

namespace Phrasefable.StardewMods.StarUnit
{
    [UsedImplicitly]
    public class StarUnit : Mod
    {
        private readonly IDictionary<string, IList<ITraversable>> TestNodes =
            new Dictionary<string, IList<ITraversable>>();

        public override void Entry(IModHelper helper) { }

        public override object GetApi()
        {
            return new StarUnitApi(this.Register)
            {
                BuilderFactory = new BuilderFactory()
            };
        }

        private void Register(string modId, params ITraversable[] testNodes)
        {
            this.TestNodes.TryGetValue(modId, out IList<ITraversable> rootNodes);
            if (rootNodes == null)
            {
                rootNodes = new List<ITraversable>();
                this.TestNodes[modId] = rootNodes;
            }

            ICollection<string> rootKeys = new HashSet<string>(rootNodes.Select(node => node.Key).ToArray());

            foreach (ITraversable newRoot in testNodes)
            {
                if (rootKeys.Contains(newRoot.Key))
                {
                    throw new ArgumentException(
                        "Cannot register tests or test suites at root level with duplicate keys.",
                        nameof(testNodes)
                    );
                }

                rootKeys.Add(newRoot.Key);
                rootNodes.Add(newRoot);
            }

            Monitor.Log($"Registered {rootNodes.Count} root test-node(s) at '{modId}'.");
        }
    }
}
