namespace Phrasefable.StardewMods.NoOre
{
    public interface IModConfig
    {
        bool ReplaceOres { get; }
        bool ReplaceGemNodes { get; }
        bool ReplaceMysticStone { get; }
        bool ReplaceGeodeNodes { get; }
    }


    public class ModConfig : IModConfig
    {
        public bool ReplaceOres { get; set; }
        public bool ReplaceGemNodes { get; set; }
        public bool ReplaceMysticStone { get; set; }
        public bool ReplaceGeodeNodes { get; set; }
    }
}
