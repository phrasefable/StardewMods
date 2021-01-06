using System;
using System.Text.RegularExpressions;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Internal.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Internal.Builders
{
    /// <summary>
    /// Internal class used as component in other builders - handles building and validation of IIdentifiable's members
    /// </summary>
    internal class IdentifiableBuilder : IBuilder<IIdentifiable>, IIdentifiableBuilder
    {
        private readonly Identifiable _identifiable;

        private static readonly string ValidKeyPattern = @"^\w+$";

        public IdentifiableBuilder(Identifiable identifiable)
        {
            this._identifiable = identifiable;
            this._key = new SettableOnce<string>(this.SetKey, nameof(IdentifiableBuilder.Key));
        }

        private readonly SettableOnce<string> _key;

        public string Key
        {
            set => this._key.Value = value;
        }

        public string LongName
        {
            set => this._identifiable.LongName = value;
        }

        private void SetKey(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(paramName: nameof(key));
            }

            if (!Regex.IsMatch(key, IdentifiableBuilder.ValidKeyPattern))
            {
                throw new ArgumentException(
                    $"An identifiable's key must match `{IdentifiableBuilder.ValidKeyPattern}`, which `{key}` does not.",
                    nameof(key)
                );
            }

            this._identifiable.Key = key;
        }

        public IIdentifiable Build()
        {
            if (!this._key.HasBeenSet) throw new InvalidOperationException("Identifiable must have a key set.");

            return this._identifiable;
        }
    }
}