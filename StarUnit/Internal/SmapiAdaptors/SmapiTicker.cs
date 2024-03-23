using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Internal.Runners;
using StardewModdingAPI.Events;

namespace Phrasefable.StardewMods.StarUnit.Internal.SmapiAdaptors
{
    internal class SmapiTicker : IDelayedExecutor
    {
        private readonly IGameLoopEvents _events;


        public SmapiTicker(IGameLoopEvents events)
        {
            this._events = events;
        }


        public Action<Action> After(Delay delay)
        {
            return delay switch
            {
                Delay.None => callback => callback(),
                Delay.Tick => this.UpdateTicked,
                Delay.Second => this.OneSecondUpdateTicked,
                _ => throw new ArgumentOutOfRangeException(nameof(delay), delay, null)
            };
        }


        private void UpdateTicked(Action callback)
        {
            EventUtilities.HandleOnceWhenPlayerFree<UpdateTickedEventArgs>(
                (_, __) => callback(),
                handler => this._events.UpdateTicked += handler,
                handler => this._events.UpdateTicked -= handler
            );
        }


        private void OneSecondUpdateTicked(Action callback)
        {
            EventUtilities.HandleOnceWhenPlayerFree<OneSecondUpdateTickedEventArgs>(
                (_, __) => callback(),
                handler => this._events.OneSecondUpdateTicked += handler,
                handler => this._events.OneSecondUpdateTicked -= handler
            );
        }
    }
}
