using Phrasefable.StardewMods.StarUnit.Api.Model;

namespace Phrasefable.StardewMods.StarUnit.Default
{
    internal class Identifiable : IIdentifiable
    {
        public string Key { get; set; }
        public string LongName { get; set; }
    }
}