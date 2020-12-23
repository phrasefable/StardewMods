using System;
using JetBrains.Annotations;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework
{
    public interface IBaseTestBuilder<out T> where T : IBaseTestBuilder<T>
    {
        [NotNull] public T Build();

        public T SetName(string name);

        public T AddCondition(Func<IResult> condition);

        public T SetBeforeAllAction(Action action);
        public T SetBeforeEachAction(Action action);
        public T SetAfterEachAction(Action action);
        public T SetAfterAllAction(Action action);
    }

    public interface ITestNodeBuilder : IBaseTestBuilder<ITestNodeBuilder>
    {
        public ITestNodeBuilder AddChildNode(Action<ITestNodeBuilder> childBuilderCallback);
        public ITestNodeBuilder AddTest(Action<ITestBuilder> childBuilderCallback);
        public ITestNodeBuilder AddCasedTest<TCaseParams>(Action<ICasedTestBuilder<TCaseParams>> childBuilderCallback);
    }

    public interface ITestableBuilder<out TThis, in T> where T : Delegate
    {
        public TThis SetTestMethod(T testMethod);
    }

    public interface ITestBuilder : IBaseTestBuilder<ITestBuilder>, ITestableBuilder<ITestBuilder, Func<IResult>>
    {
        // public ITestBuilder SetTestMethod(Func<IResult> testMethod);
    }

    // public readonly struct Case<TIn, TOut>
    // {
    //     public readonly TIn Input;
    //     public readonly TOut ExpectedOutput;
    //
    //     public Case(TIn input, TOut expectedOutput)
    //     {
    //         this.Input = input;
    //         this.ExpectedOutput = expectedOutput;
    //     }
    // }

    public interface ICasedTestBuilder<TCaseParams> : IBaseTestBuilder<ICasedTestBuilder<TCaseParams>>,
        ITestableBuilder<ICasedTestBuilder<TCaseParams>, Func<TCaseParams, IResult>>
    {
        public ICasedTestBuilder<TCaseParams> AddCase(TCaseParams caseParams);
        // public ICasedTestBuilder<TIn, TOut> SetTestMethod(Func<TIn, TOut, IResult> testMethod);
    }
}
