using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Framework
{
    public delegate void OnCompleted(ITraversableResult result);


    public interface IExecutionContext
    {
        // TODO - return an Action<OnCompleted, T> instead??? - greatly improves empty execution context.
        public void Execute<T>(OnCompleted @return, Action<OnCompleted, T> executable, T node) where T : ITraversable;
    }


    public interface IRunner
    {
        public void Run(OnCompleted @return, ITraversable node, IExecutionContext context);
        public void Skip(OnCompleted @return, ITraversable node);
        public void Skip(OnCompleted @return, ITraversable node, Status status, string message);
    }


    public interface IComponentRunner : IRunner
    {
        public bool MayHandle(ITraversable node);
    }


    public interface IRunnerDelegator : IRunner
    {
        public void Run(Action @return, Action action, Delay delay);
    }


    public interface ICompositeRunner : IRunner
    {
        public void Add(IComponentRunner component);
    }


    public interface ITreeRunner
    {
        public void Run(OnCompleted @return, ITraversable root);
    }
}
