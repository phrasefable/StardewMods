namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework
{
    public readonly struct DoubleToBool
    {
        public readonly double Double;
        public readonly bool Bool;


        public DoubleToBool(double d, bool b)
        {
            this.Double = d;
            this.Bool = b;
        }
    }

    public readonly struct StringToBool
    {
        public readonly string String;
        public readonly bool Bool;


        public StringToBool(string s, bool b)
        {
            this.String = s;
            this.Bool = b;
        }
    }
}
