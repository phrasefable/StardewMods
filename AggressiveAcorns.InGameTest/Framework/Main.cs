using System;
using JetBrains.Annotations;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework
{
    public class Main
    {
        private readonly ITestNode _root;
        // private ITestNodeBuilder _rootBuilder;

        public Main()
        {
            this._root = this.SetUpTests();
        }

        private readonly struct Params
        {
            public readonly int Input;
            public readonly bool ShouldBeEven;

            public Params(int input, bool shouldBeEven)
            {
                this.Input = input;
                this.ShouldBeEven = shouldBeEven;
            }
        }

        [UsedImplicitly]
        public ITestNode SetUpTests()
        {
            ITestNodeBuilder rootBuilder = null;

            static IResult DummyCaseTest(Params @case)
            {
                bool result = (@case.Input % 2 == 0) == @case.ShouldBeEven;
                return new Result();
            }

            rootBuilder.AddChildNode(
                childBuilder => childBuilder.SetName("seeds")
            );

            rootBuilder.AddCasedTest<Params>(
                    casedTestBuilder => casedTestBuilder
                        .SetTestMethod(DummyCaseTest)
                        .AddCase(new Params(1, false))
                )
                .AddTest(
                    testBuilder => testBuilder.SetName("not_seeds").SetTestMethod(() => new Result())
                );

            return rootBuilder.Build();
        }


        [UsedImplicitly]
        public void RunTests(string testFilter) { }


        [UsedImplicitly]
        public string ListTests(string testFilter)
        {
            throw new NotImplementedException();
        }

        private readonly char[] _delimiters = new[] {'.'};

        /// <summary>
        /// Filters a test structure
        ///
        /// </summary>
        /// <param name="root"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [CanBeNull] public ITestNode ResolveTestFilter(ITestNode root, string filter)
        {
            var splitFilter = filter.Split(this._delimiters, 2);

            if (splitFilter.Length == 0) { }
            else if (splitFilter.Length == 1) { }
            else { }

            var childKey = splitFilter[0];
        }
    }
}
