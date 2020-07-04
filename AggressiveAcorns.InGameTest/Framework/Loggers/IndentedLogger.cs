namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Loggers
{
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
}