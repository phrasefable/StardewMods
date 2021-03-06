using System;
using StardewModdingAPI;

namespace Phrasefable.StardewMods.StarUnit.Internal.SmapiAdaptors
{
    internal static class EventUtilities
    {
        public static void HandleOnceWhenPlayerFree<TArgs>(
            EventHandler<TArgs> handler,
            Action<EventHandler<TArgs>> doSubscribe,
            Action<EventHandler<TArgs>> unSubscribe
        )
        {
            void Wrapper(object sender, TArgs args)
            {
                if (!Context.IsPlayerFree) return;
                unSubscribe(Wrapper);
                handler(sender, args);
            }

            doSubscribe(Wrapper);
        }
    }
}
