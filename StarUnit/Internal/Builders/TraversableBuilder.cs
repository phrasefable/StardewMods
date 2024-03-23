using System.Text.RegularExpressions;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using Phrasefable.StardewMods.StarUnit.Internal.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Internal.Builders
{
    /// <summary>
    ///     Internal class used as component in other builders - handles building and validation of ITraversable's members
    /// </summary>
    internal class TraversableBuilder : ITraversableBuilder
    {
        private static readonly string ValidKeyPattern = @"^\w+$";

        private readonly SettableOnce<string> _key = new SettableOnce<string>(nameof(Key));
        private readonly SettableOnce<string> _longName = new SettableOnce<string>(nameof(LongName));
        private readonly ICollection<Func<IResult>> _conditions = new List<Func<IResult>>();
        private readonly SettableOnce<Delay> _delay = new SettableOnce<Delay>(nameof(Delay));


        public string Key
        {
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (!Regex.IsMatch(value, ValidKeyPattern))
                {
                    throw new ArgumentException(
                        $"A traversable node's key must match `{ValidKeyPattern}`, which `{value}` does not.",
                        nameof(value)
                    );
                }

                this._key.Value = value;
            }
        }


        public string LongName
        {
            set => this._longName.Value = value;
        }


        public Delay Delay
        {
            set => this._delay.Value = value;
        }


        public void AddCondition(Func<IResult> condition)
        {
            this._conditions.Add(condition);
        }


        public void Build(Traversable traversable)
        {
            if (!this._key.HasBeenSet) throw new InvalidOperationException("A traversable node must have a key set.");

            traversable.Key = this._key.Value;
            traversable.LongName = this._longName.Value;
            traversable.Conditions = this._conditions;
            traversable.Delay = this._delay.Value;
        }
    }
}