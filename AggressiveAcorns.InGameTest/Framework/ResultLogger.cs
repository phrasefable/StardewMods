using System.Collections.Generic;
using StardewModdingAPI;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework
{
    internal interface ILogger
    {
        public void Log(IMonitor monitor);
    }


    internal class IndentedLogger
    {
        private readonly IndentedLogger _prev;
        private IndentedLogger _next;


        public IndentedLogger In
        {
            get { return this._next ??= new IndentedLogger(this); }
        }


        protected IndentedLogger(IndentedLogger prev)
        {
            this._prev = prev;
        }


        public void Append(string @string)
        {
            this._Append(@string);
        }


        public void Append(TestResult result)
        {
            this.Append(result.ToString());
        }


        protected virtual void _Append(string @string, int indent = 0)
        {
            this._prev._Append(@string, indent + 1);
        }
    }


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
            LogLevel level = HasFailure ? LogLevel.Error : LogLevel.Info;
            this._lines.ForEach(line => monitor.Log(line, level));
        }


        protected override void _Append(string @string, int indent = 0)
        {
            this._lines.Add(new string(' ', 2 * (indent + 1)) + @string);
        }
    }
}
