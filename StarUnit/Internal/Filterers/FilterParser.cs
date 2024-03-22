using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Phrasefable.StardewMods.StarUnit.Internal.Filterers
{
    internal class FilterParser
    {
        private readonly char[] _delimiters = {'/'};
        private readonly Regex _validKeyPattern = new(@"^\w+$");


        public IEnumerable<IStringNode> BuildFilterTrees(IEnumerable<string> filters)
        {
            IEnumerable<string[]> splitFilters = filters
                .Select(fString => fString.Split(this._delimiters))
                .OrderBy(f => f.Length)
                .ToArray();

            this.ValidateFilterKeysFormat(splitFilters);

            var root = new StringNode();
            foreach (string[] filter in splitFilters)
            {
                this.CheckKeysValid(filter);
                ParseFilterString(root, filter);
            }

            return root.Children;
        }


        private void ValidateFilterKeysFormat(IEnumerable<string[]> splitFilters)
        {
            IEnumerable<string> GetInvalidFormattedKeys(IEnumerable<string> strings)
            {
                return strings.Where(s => !this._validKeyPattern.IsMatch(s));
            }

            IEnumerable<string> invalidKeys = splitFilters
                .SelectMany(GetInvalidFormattedKeys)
                .Distinct()
                .ToArray();

            if (invalidKeys.Any())
            {
                throw new ArgumentException($"Invalid keys: {string.Join(" ", invalidKeys.OrderBy(s => s))}");
            }
        }


        private static void ParseFilterString(in StringNode root, IEnumerable<string> filter)
        {
            StringNode lastNode = root;
            foreach (string childKey in filter)
            {
                if (lastNode.AllChildren) break;
                lastNode = lastNode.GetChildElseAdd(childKey);
            }

            lastNode.AllChildren = true;
        }


        private void CheckKeysValid(string[] filter)
        {
            foreach (string part in filter)
            {
                if (this._validKeyPattern.IsMatch(part)) continue;

                string fullFilter = string.Join(this._delimiters[0].ToString(), filter);
                throw new ArgumentException($"Invalid key `{part}` in `{fullFilter}`.");
            }
        }
    }
}
