using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.ResultListers
{
    internal abstract class TraversableResultLister<TResult> : IComponentResultLister<ResultListingContext>
        where TResult : ITraversableResult
    {
        protected readonly Action<string, Status> Writer;


        protected TraversableResultLister(Action<string, Status> writer)
        {
            this.Writer = writer;
        }


        protected abstract void List(TResult result, in ResultListingContext context);
        protected abstract void PreProcess(TResult result, in ResultListingContext context);


        public bool MayHandle(ITraversableResult result)
        {
            return result is TResult;
        }


        void IContextualResultLister<ResultListingContext>.List(
            ITraversableResult result,
            in ResultListingContext context)
        {
            this.List((TResult) result, context);
        }


        void IContextualResultLister<ResultListingContext>.PreProcess(
            ITraversableResult result,
            in ResultListingContext context)
        {
            this.PreProcess((TResult) result, context);
        }


        protected virtual void WriteMessage(TResult result, in ResultListingContext context)
        {
            if (string.IsNullOrWhiteSpace(result.Message)) return;

            foreach (string line in context.GetMessageLines(result.Message))
            {
                this.Writer.Invoke(line, result.Status);
            }
        }
    }
}