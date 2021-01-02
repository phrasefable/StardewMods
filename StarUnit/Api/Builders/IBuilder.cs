namespace Phrasefable.StardewMods.StarUnit.Api.Builders
{
    public interface IBuilder<out T>
    {
        public T Build();
    }
}
