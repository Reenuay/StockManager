using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using StockManager.Models;
using StockManager.Properties;

namespace StockManager.Utilities
{
    static class NameGenerator
    {
        private static Random dice = new Random();

        private static List<Func<string, Composition, string>> handlers = new List<Func<string, Composition, string>>
        {
            (t, c) =>
            {
                return new Regex(@"\[theme\]").Replace(t, c.Theme.Name);
            },
            (t, c) =>
            {
                return new Regex(@"\[themeLowerCase\]").Replace(t, c.Theme.Name.ToLower());
            },
            (t, c) =>
            {
                return new Regex(@"\[keywords\]")
                    .Replace(t, c.Keywords
                        .Select(k => k.Name)
                        .Aggregate(
                            (a,b) => a + ", " + b
                        )
                    );
            },
            (t, c) =>
            {
                return new Regex(@"\[keywordCount\]").Replace(t, c.Keywords.Count.ToString());
            },
            (t, c) =>
            {
                return new Regex(@"\[icons\]")
                    .Replace(t, c.Set.Icons
                        .Select(i => i.Name)
                        .Aggregate(
                            (a,b) => a + ", " + b
                        )
                    );
            },
            (t, c) =>
            {
                return new Regex(@"\[iconCount\]").Replace(t, c.Set.Icons.Count.ToString());
            },
            (t, c) =>
            {
                var r = new Regex(@"\[keywordsRandom:(\d+)\]");

                while (true)
                {
                    var match = r.Match(t);

                    if (!match.Success)
                        break;

                    var keywords = c.Keywords.Select(k => k.Name).ToList();
                    var n = int.Parse(match.Groups[1].Value);

                    if (n > keywords.Count)
                    {
                        n = keywords.Count;
                    }

                    var s = "";
                    var start = true;

                    while (n > 0)
                    {
                        if (!start && n != 1)
                            s += ", ";

                        start = false;

                        if (n == 1)
                            s += " and ";

                        var index = dice.Next(keywords.Count);

                        s += keywords[index];

                        keywords.RemoveAt(index);

                        n--;
                    }

                    t = t.Remove(match.Index, match.Length).Insert(match.Index, s);
                }

                return t;
            },
        };

        public static string GenerateName(Composition composition)
        {
            if (composition == null)
                throw new ArgumentNullException(nameof(composition));

            if (Settings.Default.NameTemplates.Count == 0)
                return "";

            var tepmlateString
                = Settings.Default.NameTemplates[
                    dice.Next(Settings.Default.NameTemplates.Count)
                ];

            return handlers.Aggregate(tepmlateString, (s, h) => h(s, composition));
        }
    }
}
