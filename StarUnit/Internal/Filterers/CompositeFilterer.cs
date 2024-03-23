using Phrasefable.StardewMods.StarUnit.Framework.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Internal.Filterers
{
    internal class CompositeFilterer : ICompositeFilterer
    {
        private readonly ICollection<IComponentFilterer> _components = new List<IComponentFilterer>();


        public ITraversable Filter(ITraversable node, IEnumerable<IStringNode> possibleFilterNodes)
        {
            IStringNode filter = possibleFilterNodes.FirstOrDefault(f => f.Key == node.Key);
            if (filter == null) return null;
            return filter.AllChildren
                ? node
                : this.GetFiltererFor(node).Filter(node, filter);
        }


        public void Add(IComponentFilterer component)
        {
            this._components.Add(component);
        }


        private IComponentFilterer GetFiltererFor(ITraversable traversable)
        {
            return this._components.First(c => c.MayHandle(traversable));
        }
    }
}
