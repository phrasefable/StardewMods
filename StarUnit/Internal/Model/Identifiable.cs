using Phrasefable.StardewMods.StarUnit.Framework.Model;

namespace Phrasefable.StardewMods.StarUnit.Internal
{
    internal class Identifiable : IIdentifiable
    {
        public string Key { get; set; }
        public string LongName { get; set; }
    }
}