using System;

namespace Phrasefable.StardewMods.StarUnit.Framework
{
    public enum Status
    {
        Pass,
        Fail,
        Error,
        Skipped
    }


    public static class StatusExtensions
    {
        public static string GetPrintName(this Status status)
        {
            return status switch
            {
                Status.Pass => "Passed",
                Status.Fail => "Failed",
                Status.Error => "Error",
                Status.Skipped => "Skipped",
                _ => throw new ArgumentException("Unkown Status type")
            };
        }
    }
}