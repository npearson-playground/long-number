using System;
using System.Collections.Generic;

namespace LongNumber
{
    internal ref struct NumericSegment
    {
        private static readonly string[] SingleWord = new[] {
            "one", "two", "three", "four", "five", "six", "seven", "eight",
            "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen",
            "seventeen", "eighteen", "nineteen"            
        };

        private static readonly string[] CompoundWord = new[] {
            "twenty", "thirty", "fourty", "fifty", "sixty", "seventy", "eighty", "nintety"
        };

        private static readonly string[] SegmentWord = new[] {
            "thousand", "million", "billion", "trillion", "quadrillion", "quintillion",
            "sextillion", "septillion", "octillion", "nonillion"
        };        

        public int Segment { get; init; }
        public int Hundreds { get; init; }
        public int Tens { get; init; }
        public int Ones { get; init; }

        public NumericSegment(int segment, ReadOnlySpan<char> input)
        {
            Segment = segment;
            switch (input.Length)
            {
                case 3: Hundreds = input[0]-'0'; Tens = input[1]-'0'; Ones = input[2]-'0'; break;
                case 2: Hundreds = 0; Tens = input[0]-'0'; Ones = input[1]-'0'; break;
                case 1: Hundreds = 0; Tens = 0; Ones = input[0]-'0'; break;
                default: throw new ArgumentOutOfRangeException("input", "NumericSegment must only contain one to three characters");
            }

            if (Ones < 0 || Ones > 9 || Tens < 0 || Tens > 9 || Hundreds < 0 || Hundreds > 9)
            {
                throw new ArgumentOutOfRangeException("input", "NumericSegment must only contain numeric characters");
            }
        }

        public void AppendToCollection(ICollection<string> collection)
        {
            // Converting a numeric segment to a string involves appending up to three components to the collection:
            // - Hundreds - If a non-zero value is in the Hundreds position, append the value of the hundreds position with the "hundred" modifier
            // - TensAndOnes - If there are non-zero values in the Tens and Ones positions, append either:
            //   - A compound string prefix (e.g.; "twenty")
            //   - A compound string prefix and base (e.g.; "twenty-one")
            //   - (or) A single string for the combined value of tens-and-ones for values less than 20 ("one", "thirteen")
            // - Suffix - If this isn't the right-most segment number (0), and a number was provided, append the name of the segment ("thousand", "million")

            // Skip segments that only contain zero in all places
            if (Hundreds == 0 && Tens == 0 && Ones == 0) { return; }

            AppendHundredsComponent(collection);
            AppendTensAndOnesComponent(collection);
            AppendSuffixComponent(collection);
        }

        private void AppendHundredsComponent(ICollection<string> collection)
        {
            if (Hundreds > 0)
            {
                // Hundreds is a named single digit followed by "hundred", e.g.; one hundred, nine hundred
                collection.Add(SingleWord[Hundreds-1]);
                collection.Add("hundred");
            }
        }

        private void AppendTensAndOnesComponent(ICollection<string> collection)
        {
            if (Tens == 0 && Ones == 0)
            {
                // Segment has no more numbers in the tens and ones positions, stop at "n hundred"
            }
            else if (Tens > 1 && Ones == 0)
            {
                // Compound number prefix without a base to print [20, 30, ...], e.g.; twenty, thirty, fourty
                collection.Add(CompoundWord[Tens-2]);
            }
            else if (Tens > 1)
            {
                // Compound numbers with a base [21-29, 31-39, ..91-99], e.g.; twenty-one, thirty-two, fourty-five
                string compoundNumber = string.Concat(CompoundWord[Tens-2], "-", SingleWord[Ones-1]);
                collection.Add(compoundNumber);
            }
            else
            {
                // Single word values [0-19], e.g.; one, ten, nineteen
                var value = (Tens * 10) + Ones;
                collection.Add(SingleWord[value-1]);
            }
        }

        private void AppendSuffixComponent(ICollection<string> collection)
        {
            if (Segment == 0)
            {
                // Skip appending a suffix to the hundreds segment
            }
            else if (Segment < SegmentWord.Length + 1)
            {
                // Anything beyond the first segment will have a named suffix e.g.; thousand, million, billion
                collection.Add(SegmentWord[Segment-1]);
            }
            else
            {
                // Numbers above segment 10 (i.e., nonillion) use Conway-Weschler notation
                collection.Add(ConwayWechslerNotation.GetName(Segment));
            }
        }

        public override string ToString()
        {
            var outputList = new List<string>();
            AppendToCollection(outputList);
            return string.Join(' ', outputList);
        }
    }
}