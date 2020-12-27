namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Builders
{
    public interface IBuilder<out T>
    {
        public T Build();
    }
}