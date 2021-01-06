using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Results
{
    public class Result : IResult
    {
        public Status Status { get; set; }
        public string Message { get; set; }
    }
}
