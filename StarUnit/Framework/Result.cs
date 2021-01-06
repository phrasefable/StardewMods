using Phrasefable.StardewMods.StarUnit.Framework.Model;

namespace Phrasefable.StardewMods.StarUnit.Framework
{
    public class Result : IResult
    {
        public Status Status { get; set; }
        public string Message { get; set; }

        public string Key { get; set; }
        public string LongName { get; set; }

        public Result()
        {
        }

        public Result(ITraversable traversable)
        {
            this.Key = traversable.Key;
            this.LongName = traversable.LongName;
        }
    }
}
