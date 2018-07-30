using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace StockManager.Utilities {
    static class StringFunctions {
        public static string TakeLastLines(this string text, int count) {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            var lines = new List<string>();
            var match = Regex.Match(
                text, "^.*$",
                RegexOptions.Multiline | RegexOptions.RightToLeft
            );

            while (match.Success && lines.Count < count) {
                lines.Add(match.Value);
                match = match.NextMatch();
            }

            lines.Reverse();

            return lines.Aggregate((a, b) => a + "\n" + b);
        }

        public static string TakeFirstLines(this string text, int count) {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            var lines = new List<string>();
            var match = Regex.Match(
                text, "^.*$",
                RegexOptions.Multiline
            );

            while (match.Success && lines.Count < count) {
                lines.Add(match.Value);
                match = match.NextMatch();
            }

            return lines.Aggregate((a, b) => a + "\n" + b);
        }
    }
}
