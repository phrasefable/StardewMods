namespace Phrasefable.StardewMods.StarUnit.Internal.ResultListers
{
    internal class ColumnWidthInfo
    {
        public int Column1 { get; private set; }
        public int TotalsColumn { get; private set; }


        public void UpdateCol1(int value)
        {
            if (value > this.Column1) this.Column1 = value;
        }


        public void UpdateTotalsColumn(int value)
        {
            if (value > this.TotalsColumn) this.TotalsColumn = value;
        }
    }
}
