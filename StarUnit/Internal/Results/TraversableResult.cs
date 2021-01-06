using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Results
{
    internal class TraversableResult : Result, ITraversableResult
    {
        public string Key { get; set; }
        public string LongName { get; set; }
    }
}
