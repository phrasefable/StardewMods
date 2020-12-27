using System;
using System.Collections.Generic;
using System.Linq;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Builders
{
    public class Validator : IValidator
    {
        private readonly List<KeyValuePair<Type, Action<object>>> _validators =
            new List<KeyValuePair<Type, Action<object>>>();


        public void AddValidation<T>(Action<T> validator)
        {
            this._validators.Add(
                new KeyValuePair<Type, Action<object>>(
                    typeof(T),
                    actuallyT => validator((T) actuallyT)
                )
            );
        }


        public void Validate<T>(T subject)
        {
            foreach (KeyValuePair<Type, Action<object>> pair in this._validators.Where(pair => pair.Key == typeof(T)))
            {
                pair.Value(subject);
            }
        }
    }
}