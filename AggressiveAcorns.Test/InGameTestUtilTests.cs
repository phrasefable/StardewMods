using System;
using FluentAssertions;
using NUnit.Framework;

namespace Phrasefable.StardewMods.AggressiveAcorns.Test
{
    [TestFixture]
    public class InGameTestUtilsWithValueTests
    {
        private const string String1 = "string";
        private Func<string> _mutated;

        [SetUp]
        public void SetUp()
        {
            _mutated = () => InGameTestUtilsWithValueTests.String1;
        }

        [Test]
        public void Test_InitialValue()
        {
            this._mutated.Invoke().Should().Be(InGameTestUtilsWithValueTests.String1);
        }

        [Test]
        public void Test_Value()
        {
            var theValue = $"{InGameTestUtilsWithValueTests.String1}-(not any more)";
            this.WithValue(theValue).Should().Be(theValue);
        }

        [Test]
        public void Test_ValueAfter()
        {
            string valueBefore = this._mutated.Invoke();
            string _ = this.WithValue("doesn't matter");
            this._mutated.Invoke().Should().Be(valueBefore);
        }

        private string WithValue(string newValue)
        {
            return InGameTest.Utils.WithValue(ref this._mutated, () => newValue, this._mutated);
        }
    }
}
