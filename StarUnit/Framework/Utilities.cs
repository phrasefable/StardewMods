using StardewModdingAPI;

namespace Phrasefable.StardewMods.StarUnit.Framework
{
    public static class Conditions
    {
        public static Result WorldReady()
        {
            return Context.IsWorldReady
                ? new Result(Status.Pass)
                : new Result(Status.Fail, "World not ready.");
        }
    }
}