using System;
using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Runners
{
    internal static class Extensions
    {
        public static void ForEach(
            this IEnumerable<ITraversable> elements,
            Action callback,
            Action<OnCompleted, ITraversable> transform,
            Action<ITraversableResult> consume
        )
        {
            IEnumerator<ITraversable> enumerator = elements.GetEnumerator();

            void ConsumeTransformed(ITraversableResult transformed)
            {
                consume(transformed);
                NextElement();
            }

            void NextElement()
            {
                if (enumerator.MoveNext())
                {
                    transform(ConsumeTransformed, enumerator.Current);
                }
                else
                {
                    enumerator.Dispose();
                    callback();
                }
            }

            NextElement();
        }
    }
}
