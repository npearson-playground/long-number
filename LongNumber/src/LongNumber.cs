using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace LongNumber
{
    public static class LongNumber
    {
        private static Regex currencyRegEx = new(string.Concat(
            @"^",                                           // Matching begins at start of string
            @"(?<negativeSign>-)?",                         // The "-" sign MAY occur as the first character to denote a negative amount
            @"\$?",                                         // The "$" sign MAY occur after the optional negative sign; implied if missing
            @"(?<dollarAmount>\d+|(\d{1,3}(?:,\d{3})*))",   // A sequence of ONE OR MORE numbers MUST occur next to denote the dollar amount
                                                            // The sequence MAY contain commas, but are expected to occur every three digits if used
            @"(\.(?<fractionalAmount>\d{2}))?",             // A fractional amount (cents) MAY be specified but MUST be a period followed by TWO digits
            @"$"                                            // Matching ends at end of string
        ), RegexOptions.ExplicitCapture);

        public static string ConvertToLongForm(string numericString)
        {
            var output = new List<string>();
            foreach (var segment in new NumericSegmentEnumerator(numericString))
            {
                segment.AppendToCollection(output);
            }

            if (output.Count == 0)
            {
                return "Zero";
            }
            else
            {
                CapitalizeFirstWord(output);
                return string.Join(' ', output);
            }
        }

        public static string ConvertCurrencyToLongForm(string currencyString)
        {
            var match = currencyRegEx.Match(currencyString);
            if (!match.Success)
            {
                throw new ArgumentException("Currency value needs to be in the form of -$0.00 (negative) or $0.00 (positive)", "currencyString");
            }

            var output = new List<string>();
            
            if (match.Groups.TryGetValue("negativeSign", out var negativeSignGroup))
            {
                if (negativeSignGroup.Value == "-")
                {
                    output.Add("negative");
                }
            }
            
            bool hasNonZeroDollarAmount = true;
            if (match.Groups.TryGetValue("dollarAmount", out var dollarAmountGroup))
            {
                var dollarAmount = dollarAmountGroup.Value.Replace(",", string.Empty).TrimStart('0');
                if (dollarAmount.Length == 0)
                {
                    hasNonZeroDollarAmount = false;
                }
                else if (dollarAmount == "1")
                {
                    output.Add("one dollar");
                }
                else
                {
                    foreach (var segment in new NumericSegmentEnumerator(dollarAmount))
                    {
                        segment.AppendToCollection(output);
                    }
                    output.Add("dollars");
                }
            }

            if (match.Groups.TryGetValue("fractionalAmount", out var fractionalAmountGroup))
            {
                var fractionalAmount = fractionalAmountGroup.Value.TrimStart('0');
                if (fractionalAmount.Length != 0)
                {
                    if (hasNonZeroDollarAmount)
                    {
                        output.Add("and");
                    }

                    if (fractionalAmount == "1")
                    {
                        output.Add("one cent");
                    }
                    else
                    {
                        var segment = new NumericSegment(0, fractionalAmount);
                        segment.AppendToCollection(output);
                        output.Add("cents");
                    }
                }
            }

            if (output.Count == 0)
            {
                return "Zero dollars";
            }
            else
            {
                CapitalizeFirstWord(output);
                return string.Join(' ', output);
            }
        }

        private static void CapitalizeFirstWord(List<string> words)
        {
            if (words.Count == 0) return;

            string firstWord = words[0];
            firstWord = string.Concat(char.ToUpper(firstWord[0]), firstWord.Substring(1));
            words[0] = firstWord;
        }
    }
}
