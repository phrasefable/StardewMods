using System.Collections.Generic;
using StardewModdingAPI;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Loggers
{
    internal class AggregateLogger : ILogger
    {
        private readonly List<ILogger> _loggers = new List<ILogger>();


        public void Add(ILogger logger)
        {
            this._loggers.Add(logger);
        }


        public void Log(IMonitor monitor)
        {
            this._loggers.ForEach(item => item.Log(monitor));
        }
    }
}