using System;
using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Model;

namespace Phrasefable.StardewMods.StarUnit.Internal.Builders
{
    internal class TestFixtureBuilder : ITestFixtureBuilder
    {
        private readonly TestSuite _fixture;

        private readonly IdentifiableBuilder _identifiableBuilder;

        private readonly IList<Func<Result>> _conditions;

        private readonly SettableOnce<Action> _beforeAll;
        private readonly SettableOnce<Action> _beforeEach;
        private readonly SettableOnce<Action> _afterEach;
        private readonly SettableOnce<Action> _afterAll;

        private readonly BranchChildrenBuilder<ITraversable> _branchChildrenBuilder;


        public TestFixtureBuilder()
        {
            this._fixture = new TestSuite();

            this._identifiableBuilder = new IdentifiableBuilder(this._fixture);

            this._conditions = new List<Func<Result>>();

            this._beforeAll = new SettableOnce<Action>(nameof(TestFixtureBuilder.BeforeAll));
            this._beforeEach = new SettableOnce<Action>(nameof(TestFixtureBuilder.BeforeEach));
            this._afterEach = new SettableOnce<Action>(nameof(TestFixtureBuilder.AfterEach));
            this._afterAll = new SettableOnce<Action>(nameof(TestFixtureBuilder.AfterAll));

            this._branchChildrenBuilder = new BranchChildrenBuilder<ITraversable>();
        }

        public ITestSuite Build()
        {
            this._identifiableBuilder.Build();

            this._fixture.Conditions = this._conditions;

            this._fixture.BeforeAll = this._beforeAll.Value;
            this._fixture.BeforeEach = this._beforeEach.Value;
            this._fixture.AfterEach = this._afterEach.Value;
            this._fixture.AfterAll = this._afterAll.Value;

            this._fixture.Children = this._branchChildrenBuilder.Build();

            return this._fixture;
        }

        public string Key
        {
            set => this._identifiableBuilder.Key = value;
        }

        public string LongName
        {
            set => this._identifiableBuilder.LongName = value;
        }

        public void AddCondition(Func<Result> condition)
        {
            this._conditions.Add(condition);
        }

        public Action BeforeAll
        {
            set => this._beforeAll.Value = value;
        }

        public Action BeforeEach
        {
            set => this._beforeEach.Value = value;
        }

        public Action AfterEach
        {
            set => this._afterEach.Value = value;
        }

        public Action AfterAll
        {
            set => this._afterAll.Value = value;
        }

        public void AddChild(ITraversable child)
        {
            this._branchChildrenBuilder.AddChild(child);
        }
    }
}