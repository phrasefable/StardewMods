using System;
using System.Text.RegularExpressions;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Internal.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Internal.Builders
{
    /// <summary>
    /// Internal class used as component in other builders - handles building and validation of IIdentifiable's members
    /// </summary>
    internal class IdentifiableBuilder : IIdentifiableBuilder
    {
        private static readonly string ValidKeyPattern = @"^\w+$";

        private readonly SettableOnce<string> _key;
        private readonly SettableOnce<string> _longName;

        public IdentifiableBuilder()
        {
            this._key = new SettableOnce<string>(nameof(IdentifiableBuilder.Key));
            this._longName = new SettableOnce<string>(nameof(IdentifiableBuilder.LongName));
        }

        public string Key
        {
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(paramName: nameof(value));
                }

                if (!Regex.IsMatch(value, IdentifiableBuilder.ValidKeyPattern))
                {
                    throw new ArgumentException(
                        $"An identifiable's key must match `{IdentifiableBuilder.ValidKeyPattern}`, which `{value}` does not.",
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

        public void Build(Traversable identifiable)
        {
            if (!this._key.HasBeenSet) throw new InvalidOperationException("Identifiable must have a key set.");

            identifiable.Key = this._key.Value;
            identifiable.LongName = this._longName.Value;
        }
    }
}
