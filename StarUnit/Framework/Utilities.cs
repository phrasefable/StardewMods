using StardewModdingAPI;

namespace Phrasefable.StardewMods.StarUnit.Framework
{
    public static class Conditions
    {
        public static IResult WorldReady()
        {
            return Context.IsWorldReady
                ? new Result{Status = Status.Pass}
                : new Result{Status = Status.Fail, Message = "World not ready."};
        }
    }
}
