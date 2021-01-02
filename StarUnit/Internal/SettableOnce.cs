using System;
using JetBrains.Annotations;

namespace Phrasefable.StardewMods.StarUnit.Internal
{
    internal class SettableOnce<T>
    {
        private T _value;
        private readonly string _name;

        [CanBeNull] private readonly Action<T> _passThroughSetter;

        public SettableOnce(Action<T> passThroughSetter, string name = null)
        {
            this._passThroughSetter = passThroughSetter;
            this._name = name;
        }

        public SettableOnce(string name)
        {
            this._name = name;
        }

        public bool HasBeenSet { get; private set; }

        public T Value
        {
            get => this._value;
            set
            {
                if (this.HasBeenSet)
                {
                    throw this._name == null
                        ? new InvalidOperationException()
                        : new InvalidOperationException($"`{this._name}` is only settable once.");
                }

                this._passThroughSetter?.Invoke(value);
                this._value = value;
                this.HasBeenSet = true;
            }
        }
    }
}