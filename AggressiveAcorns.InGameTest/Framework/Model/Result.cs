namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model
{
    public class Result : IResult
    {
        public Status Status { get; }
        public string Message { get; }


        public Result(Status status, string message = null)
        {
            this.Status = status;
            this.Message = message;
        }


        // public override string ToString()
        // {
        //     string s = Status.Name();
        //     if (Message != null) s += ": " + this.Message;
        //     return s;
        // }
    }
}
