using System;
using System.Collections.Generic;
using System.Linq;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.ResultListers
{
    internal class ResultListingContext
    {
        private readonly int _indentationPerLevel = 3;


        public ResultListingContext()
        {
            this.ColumnWidths = new ColumnWidthInfo();
            this.Level = 0;
        }


        private ResultListingContext(ResultListingContext parentContext)
        {
            this.ColumnWidths = parentContext.ColumnWidths;
            this.Level = parentContext.Level + 1;
        }


        public ColumnWidthInfo ColumnWidths { get; }
        private int Level { get; }


        public ResultListingContext CreateChildContext()
        {
            return new ResultListingContext(this);
        }


        public string GetColumn1(ITraversableResult result)
        {
            return this.GetPrefix() + result.Key;
        }


        public IEnumerable<string> GetMessageLines(string message)
        {
            return message
                .Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None)
                .Select(this.GetMessageLine);
        }


        private string GetMessageLine(string line)
        {
            return $" > {this.GetPrefix(this.Level + 1)}{line}";
        }


        private string GetPrefix(int? level = null)
        {
            return new string(' ', (level ?? this.Level) * this._indentationPerLevel);
        }
    }
}