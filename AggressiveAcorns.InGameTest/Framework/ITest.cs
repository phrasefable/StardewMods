using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework
{
    public interface ITestNode
    {
        [NotNull] public string Key { get; }
        [CanBeNull] public string Name { get; }

        [CanBeNull] public ITestNode Parent { get; }
        [NotNull] public IEnumerable<ITestNode> Children { get; }

        [NotNull] public IEnumerable<ITest> Conditions { get; }

        [CanBeNull] public Action BeforeAll { get; }
        [CanBeNull] public Action BeforeEach { get; }
        [CanBeNull] public Action AfterEach { get; }
        [CanBeNull] public Action AfterAll { get; }

        // bool TreatChildrenAsChildrenOfParent { get; }
    }


    public interface ITest : ITestNode
    {
        [NotNull] public IResult Invoke();
    }

    public enum ResultStatus
    {
        Pass,
        Fail,
        Error
    }

    public interface IResult
    {
        public ResultStatus status { get; }
    }

    public class Result : IResult { }

    public class TestNode : ITestNode
    {
        private readonly List<ITestNode> _children = new List<ITestNode>();
        private readonly List<ITest> _conditions = new List<ITest>();

        public string Name { get; } = null;

        public ITestNode Parent { get; } = null;

        public IEnumerable<ITestNode> Children => this._children;

        public IEnumerable<ITest> Conditions => this._conditions;

        public Action BeforeAll { get; set; }
        public Action BeforeEach { get; set; }
        public Action AfterEach { get; set; }
        public Action AfterAll { get; set; }

        // public bool TreatChildrenAsChildrenOfParent { get; set; }
    }
}
