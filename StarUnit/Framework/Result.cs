namespace Phrasefable.StardewMods.StarUnit.Framework
{
    public class Result
    {
        public Status Status { get; }
        public string Message { get; }

        public Result(Status status, string message = null)
        {
            this.Status = status;
            this.Message = message;
        }
    }
}
