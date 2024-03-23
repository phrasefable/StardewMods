using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.ResultListers
{
    internal class BranchResultLister : TraversableResultLister<IBranchResult>
    {
        private readonly IContextualResultLister<ResultListingContext> _childLister;


        public BranchResultLister(
            Action<string, Status> writer,
            IContextualResultLister<ResultListingContext> childLister
        ) : base(writer)
        {
            this._childLister = childLister;
        }


        protected override void List(IBranchResult result, in ResultListingContext context)
        {
            this.Writer(BranchResultLister.BuildLine(result, context), result.Status);

            this.WriteMessage(result, context);

            if (BranchResultLister.ShouldListChildren(result))
            {
                foreach (ITraversableResult cResult in result.Children)
                {
                    this._childLister.List(cResult, context.CreateChildContext());
                }
            }
        }


        private static bool ShouldListChildren(IBranchResult result)
        {
            if (result.Status == Status.Skipped) return false;
            if (result.DescendantLeafTallies.ContainsKey(Status.Pass) &&
                result.TotalDescendantLeaves == result.DescendantLeafTallies[Status.Pass])
            {
                return false;
            }

            return true;
        }


        private static string BuildLine(IBranchResult result, ResultListingContext context)
        {
            string buffer = new string(' ', Math.Max(1, context.ColumnWidths.TotalsColumn));
            return string.Join(buffer, BranchResultLister.BuildColumns(result, context));
        }


        private static IEnumerable<string> BuildColumns(IBranchResult result, ResultListingContext context)
        {
            yield return context.GetColumn1(result).PadRight(context.ColumnWidths.Column1);

            yield return BranchResultLister.GetTallyColumn(
                "Total",
                result.TotalDescendantLeaves.ToString(),
                context.ColumnWidths
            );

            foreach (Status status in new[] { Status.Pass, Status.Fail, Status.Skipped, Status.Error })
            {
                yield return BranchResultLister.GetTallyColumn(
                    status.GetPrintName(),
                    BranchResultLister.GetTally(result, status),
                    context.ColumnWidths
                );
            }
        }


        private static string GetTallyColumn(string name, string value, ColumnWidthInfo widthInfo)
        {
            return $"{name}: {value.PadLeft(widthInfo.TotalsColumn)}";
        }


        private static string GetTally(IBranchResult result, Status status)
        {
            return result.DescendantLeafTallies.ContainsKey(status)
                ? result.DescendantLeafTallies[status].ToString()
                : "-";
        }


        protected override void PreProcess(IBranchResult result, in ResultListingContext context)
        {
            // Done before children, as children unlikely to override
            context.ColumnWidths.UpdateTotalsColumn(BranchResultLister.CalcNumDigits(result.TotalDescendantLeaves));

            foreach (ITraversableResult child in result.Children)
            {
                this._childLister.PreProcess(child, context.CreateChildContext());
            }

            // Done after children, as less likely to override children
            context.ColumnWidths.UpdateCol1(context.GetColumn1(result).Length);
        }


        private static int CalcNumDigits(int i)
        {
            return (int) Math.Log10(i) + 1;
        }
    }
}