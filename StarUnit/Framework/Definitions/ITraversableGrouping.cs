namespace Phrasefable.StardewMods.StarUnit.Framework.Definitions
{
    /// <summary>
    ///     Represents a collection of traversable elements that should be considered direct children of the grouping's
    ///     parent when it is run, but as children of the grouping when results are reported.
    /// </summary>
    /// <remarks>
    ///     Used for cased test, where the cases are children of the cased test's parent, but we want the children
    ///     to be reported on as a group.
    /// </remarks>
    public interface ITraversableGrouping : ITraversableBranch { }
}