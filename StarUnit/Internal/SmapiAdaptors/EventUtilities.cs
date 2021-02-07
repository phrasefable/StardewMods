using System;

namespace Phrasefable.StardewMods.StarUnit.Internal.SmapiAdaptors
{
    internal static class EventUtilities
    {
        public static void HandleOnce<TArgs>(
            EventHandler<TArgs> handler,
            Action<EventHandler<TArgs>> doSubscribe,
            Action<EventHandler<TArgs>> unSubscribe
        )
        {
            void Wrapper(object sender, TArgs args)
            {
                unSubscribe(Wrapper);
                handler(sender, args);
            }

            doSubscribe(Wrapper);
        }
    }
}
