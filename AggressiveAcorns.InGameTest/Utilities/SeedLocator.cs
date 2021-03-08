using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Phrasefable.StardewMods.AggressiveAcorns.Framework;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Utilities
{
    internal class SeedLocator
    {
        public readonly ICollection<Vector2> GeneratedOffsets = new List<Vector2>();

        public IEnumerable<Vector2> GenerateOffsets()
        {
            Vector2[] offsets;
            do
            {
                offsets = AggressiveTree.GenerateSpreadOffsets().ToArray();
            } while (offsets.Any(offset => offset == Vector2.Zero));

            foreach (Vector2 offset in offsets)
            {
                this.GeneratedOffsets.Add(offset);
            }

            return offsets;
        }
    }
}
