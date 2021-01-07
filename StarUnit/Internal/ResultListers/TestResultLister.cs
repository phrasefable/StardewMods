using System;
using System.Text;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.ResultListers
{
    internal class TestResultLister : TraversableResultLister<ITestResult>
    {
        public TestResultLister(Action<string, Status> writer) : base(writer) { }


        protected override void List(ITestResult result, in ResultListingContext context)
        {
            var builder = new StringBuilder(context.GetColumn1(result));
            if (!string.IsNullOrWhiteSpace(result.LongName))
            {
                builder.Append($" - {result.LongName}");
            }

            builder.Append(" - ").Append(result.Status.GetPrintName());

            this.Writer.Invoke(builder.ToString(), result.Status);
            this.WriteMessage(result, context);
        }


        protected override void PreProcess(ITestResult result, in ResultListingContext context)
        {
            context.ColumnWidths.UpdateCol1(context.GetColumn1(result).Length);
        }
    }
}