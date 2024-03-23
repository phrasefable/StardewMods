using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using StardewModdingAPI;

namespace Phrasefable.StardewMods.ModdingTools
{
    internal enum ToggleAction
    {
        Enable,
        Disable,
        Toggle
    }


    internal class ToggleableEventHandler<TArgs> where TArgs : EventArgs
    {
        public bool IsEnabled { get; private set; }
        private readonly Action<object, TArgs> _handler;


        public ToggleableEventHandler(Action<object, TArgs> handler)
        {
            this._handler = handler;
        }


        public void OnEvent(object sender, TArgs args)
        {
            if (this.IsEnabled) this._handler.Invoke(sender, args);
        }


        public void Set(ToggleAction action)
        {
            this.IsEnabled = action switch
            {
                ToggleAction.Enable => true,
                ToggleAction.Disable => false,
                ToggleAction.Toggle => !this.IsEnabled,
                _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
            };
        }
    }


    internal interface IToggleableEventLogger
    {
        bool IsEnabled { get; }

        string Id { get; }

        void Set(ToggleAction action);
    }


    internal class ToggleableEventLogger<TArgs> : ToggleableEventHandler<TArgs>, IToggleableEventLogger
        where TArgs : EventArgs
    {
        public string Id { get; }


        public ToggleableEventLogger([NotNull] string id, IMonitor monitor, Func<TArgs, string> message)
            : base((s, args) => monitor.Log(message(args), LogLevel.Info))
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException($"invalid event logger id: `{id}`");
            }

            this.Id = id;
        }
    }


    internal class ToggleableEventLoggerCollection : IEnumerable<IToggleableEventLogger>
    {
        private readonly IDictionary<string, IToggleableEventLogger> _loggers =
            new Dictionary<string, IToggleableEventLogger>();

        public IEnumerator<IToggleableEventLogger> GetEnumerator() => this._loggers.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) this._loggers.Values).GetEnumerator();


        public void Add([NotNull] IToggleableEventLogger item)
        {
            this._loggers[item.Id] = item;
        }


        public void Set([NotNull] IEnumerable<string> loggers, ToggleAction action)
        {
            foreach (string logger in loggers)
            {
                this._loggers[logger].Set(action);
            }
        }


        [NotNull] public IEnumerable<string> Ids => this._loggers.Keys;
    }
}
