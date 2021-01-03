namespace Phrasefable.StardewMods.StarUnit.Framework
{
    public class DoubleToBool
    {
        public double Double { get; }
        public bool Bool { get; }

        public DoubleToBool(double @double, bool @bool)
        {
            this.Double = @double;
            this.Bool = @bool;
        }
    }

    public class StringToBool
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