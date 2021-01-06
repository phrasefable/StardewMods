using System;
using System.Collections.Generic;
using System.Linq;
using Phrasefable.StardewMods.StarUnit.Framework.Model;

namespace Phrasefable.StardewMods.StarUnit.Internal.TestListers
{
    internal class ConciseLister : ILister
    {
        private readonly Action<string> _writer;

        private IList<NodeInfo> _toGoOut;

        public ConciseLister(Action<string> writer)
        {
            this._writer = writer;
        }

        public void List(ITraversable node)
        {
            this._toGoOut = new List<NodeInfo>();
            this.Process(node, 0);
            while (this._toGoOut.Any())
            {
                int idxLast = this._toGoOut.Count - 1;
                NodeInfo info = this._toGoOut[idxLast];
                this._toGoOut.RemoveAt(idxLast);

                if (info.IsLeaf) continue;

                string line = new string(' ', info.Level * 3);
                if (info.Node != null) line += info.Node.Key + " ";
                line += $"({info.DescendantLeaves} test";
                if (info.DescendantLeaves > 1) line += "s";
                line += ")";

                this._writer(line);
            }
        }

        private NodeInfo Process(IIdentifiable node, int level)
        {
            var info = new NodeInfo
            {
                Node = node,
                Level = level,
                IsLeaf = true
            };

            if (node is ITestSuite suite)
            {
                info.IsLeaf = false;
                foreach (NodeInfo child in suite.Children.Select(child => this.Process(child, level + 1)))
                {
                    if (child.IsLeaf)
                    {
                        info.ChildLeaves += 1;
                        info.DescendantLeaves += 1;
                    }
                    else
                    {
                        info.DescendantLeaves += child.DescendantLeaves;
                    }
                }
            }

            this._toGoOut.Add(info);
            return info;
        }

        private struct NodeInfo
        {
            public IIdentifiable Node { get; set; }
            public int Level { get; set; }

            public bool IsLeaf { get; set; }

            /// <summary>
            /// Includes direct and indirect descendants
            /// </summary>
            public int DescendantLeaves { get; set; }

            public int ChildLeaves { get; set; }
        }
    }
}
