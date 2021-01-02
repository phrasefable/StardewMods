namespace Phrasefable.StardewMods.StarUnit.Framework.Builders
{
    public interface IBuilder<out T>
    {
        public T Build();
    }
}
