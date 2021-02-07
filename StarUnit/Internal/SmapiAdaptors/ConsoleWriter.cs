using System;
using Phrasefable.StardewMods.StarUnit.Framework;
using StardewModdingAPI;

namespace Phrasefable.StardewMods.StarUnit.Internal.SmapiAdaptors
{
    internal class ConsoleWriter
    {
        private readonly IMonitor _monitor;


        public ConsoleWriter(IMonitor monitor)
        {
            this._monitor = monitor;
        }


        public void WriteToConsole(string message, Status status)
        {
            this._monitor.Log(
                message,
                status switch
                {
                    Status.Pass => LogLevel.Info,
                    Status.Fail => LogLevel.Warn,
                    Status.Error => LogLevel.Warn,
                    Status.Skipped => LogLevel.Warn,
                    _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
                }
            );
        }
    }
}