using System;

namespace Phrasefable.StardewMods.StarUnit.Internal
{
    internal class SettableOnce<T>
    {
        private T _value;
        private readonly string _name;

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

                this._value = value;
                this.HasBeenSet = true;
            }
        }
    }
}
