namespace Phrasefable.StardewMods.StarUnit.Framework
{
    public readonly struct DoubleToBool
    {
        public double Double { get; }
        public bool Bool { get; }

        public DoubleToBool(double @double, bool @bool)
        {
            this.Double = @double;
            this.Bool = @bool;
        }
    }


    public readonly struct StringToBool
    {
        public string String { get; }
        public bool Bool { get; }

        public StringToBool(string @string, bool @bool)
        {
            this.String = @string;
            this.Bool = @bool;
        }
    }
}