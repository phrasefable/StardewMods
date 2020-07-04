using System.Collections.Generic;
using StardewModdingAPI;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Loggers
{
    internal class ResultLogger : IndentedLogger, ILogger
    {
        private readonly List<string> _lines = new List<string>();
        public bool HasFailure { get; set; } = true;


        public ResultLogger(ITest test) : base(null)
        {
            this._lines.Add($"Test: {test.Name}");
        }


        public void Log(IMonitor monitor)
        {
            LogLevel level = this.HasFailure ? LogLevel.Error : LogLevel.Info;
            this._lines.ForEach(line => monitor.Log(line, level));
        }


        protected override void _Append(string @string, int indent = 0)
        {
            this._lines.Add(new string(' ', 2 * (indent + 1)) + @string);
        }
    }
}