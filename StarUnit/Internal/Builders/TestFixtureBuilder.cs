using System;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using Phrasefable.StardewMods.StarUnit.Internal.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Internal.Builders
{
    internal class TestFixtureBuilder : ITestFixtureBuilder
    {
        private readonly TraversableBranchBuilder _branchBuilder;

        private readonly ActionSettable _beforeAll;
        private readonly ActionSettable _beforeEach;
        private readonly ActionSettable _afterEach;
        private readonly ActionSettable _afterAll;

        public TestFixtureBuilder()
        {
            this._branchBuilder = new TraversableBranchBuilder();

            this._beforeAll = new ActionSettable(
                nameof(TestFixtureBuilder.BeforeAll),
                nameof(TestFixtureBuilder.BeforeAllDelay)
            );
            this._beforeEach = new ActionSettable(
                nameof(TestFixtureBuilder.BeforeEach),
                nameof(TestFixtureBuilder.BeforeEachDelay)
            );
            this._afterEach = new ActionSettable(
                nameof(TestFixtureBuilder.AfterEach),
                nameof(TestFixtureBuilder.AfterEachDelay)
            );
            this._afterAll = new ActionSettable(
                nameof(TestFixtureBuilder.AfterAll),
                nameof(TestFixtureBuilder.AfterAllDelay)
            );
        }


        public ITestSuite Build()
        {
            var suite = new TestSuite();

            this._branchBuilder.Build(suite);

            suite.BeforeAll = this._beforeAll.Build();
            suite.BeforeEach = this._beforeEach.Build();
            suite.AfterEach = this._afterEach.Build();
            suite.AfterAll = this._afterAll.Build();

            return suite;
        }


        public string Key
        {
            set => this._branchBuilder.Key = value;
        }


        public string LongName
        {
            set => this._branchBuilder.LongName = value;
        }


        public void AddCondition(Func<IResult> condition)
        {
            this._branchBuilder.AddCondition(condition);
        }


        public Delay Delay
        {
            set => this._branchBuilder.Delay = value;
        }


        public Action BeforeAll
        {
            set => this._beforeAll.Action = value;
        }


        public Action BeforeEach
        {
            set => this._beforeEach.Action = value;
        }


        public Action AfterEach
        {
            set => this._afterEach.Action = value;
        }


        public Action AfterAll
        {
            set => this._afterAll.Action = value;
        }


        public void AddChild(ITraversable child)
        {
            this._branchBuilder.AddChild(child);
        }


        public Delay BeforeAllDelay
        {
            set => this._beforeAll.Delay = value;
        }


        public Delay BeforeEachDelay
        {
            set => this._beforeEach.Delay = value;
        }


        public Delay AfterEachDelay
        {
            set => this._afterEach.Delay = value;
        }


        public Delay AfterAllDelay
        {
            set => this._afterAll.Delay = value;
        }


        private class FixtureAction : IAction
        {
            public Action Action { get; set; }
            public Delay Delay { get; set; }
        }


        private class ActionSettable : IBuilder<IAction>
        {
            private readonly SettableOnce<Action> _action;
            private readonly SettableOnce<Delay> _delay;

            public ActionSettable(string actionName, string delayName)
            {
                this._action = new SettableOnce<Action>(actionName);
                this._delay = new SettableOnce<Delay>(delayName);
            }

            public Action Action
            {
                set => this._action.Value = value;
            }

            public Delay Delay
            {
                set => this._delay.Value = value;
            }

            public IAction Build()
            {
                if (this._action.HasBeenSet)
                {
                    return new FixtureAction { Action = this._action.Value, Delay = this._delay.Value };
                }

                if (this._delay.HasBeenSet)
                {
                    throw new InvalidOperationException("May not set delay without setting an action.");
                }

                return null;
            }
        }
    }
}