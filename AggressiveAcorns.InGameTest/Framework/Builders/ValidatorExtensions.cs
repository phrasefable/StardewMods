using System;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Builders
{
    public static class ValidatorExtensions
    {
        public static void AddIdentifiableValidation(this Validator validator)
        {
            validator.AddValidation<IIdentifiable>(
                identifiable =>
                {
                    if (identifiable.Key == null)
                    {
                        throw new InvalidOperationException("Identifiable objects must have a key set.");
                    }
                }
            );
        }
    }
}