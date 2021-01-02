namespace Phrasefable.StardewMods.StarUnit.Api
{
    public class Result
    {
        public Status Status { get; }
        public string Message { get; }

        public Result(Status status, string message)
        {
            this.Status = status;
            this.Message = message;
        }
    }
}