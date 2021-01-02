using System;
using System.Collections.Generic;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Builders
{
    public class TestFixtureBuilder : ITestFixtureBuilder
    {
        private readonly TestSuite _fixture;

        private readonly IList<Func<IResult>> _conditions = new List<Func<IResult>>();

        private readonly IList<TestNode> _children = new List<TestNode>();

        private readonly IValidator _validator;


        public TestFixtureBuilder(IValidator validator)
        {
            _fixture = new TestSuite
            {
                Conditions = this._conditions,
                Children = this._children
            };

            this._validator = validator;
        }


        public ITestSuite Build()
        {
            this._validator.Validate<IIdentifiable>(this._fixture);
            this._validator.Validate<IConditional>(this._fixture);

            return this._fixture;
        }


        public void SetKey(string value)
        {
            this._fixture.Key = value;
        }


        public void SetLongName(string value)
        {
            this._fixture.LongName = value;
        }


        public void AddCondition(Func<IResult> condition)
        {
            this._conditions.Add(condition);
        }


        public void SetBeforeAllAction(Action value)
        {
            this._fixture.BeforeAll = value;
        }


        public void SetBeforeEachAction(Action action)
        {
            this._fixture.BeforeEach = action;
        }


        public void SetAfterEachAction(Action action)
        {
            this._fixture.AfterEach = action;
        }


        public void SetAfterAllAction(Action action)
        {
            this._fixture.AfterAll = action;
        }


        public void AddChild(ITestSuite child)
        {
            this._children.Add(new TestNode.SuiteWrapper(child));
        }


        public void AddChild(ITest child)
        {
            this._children.Add(new TestNode.TestWrapper(child));
        }
    }
}