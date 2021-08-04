using System;
using System.Collections.Generic;

namespace LongNumber
{
    internal static class ConwayWechslerNotation
    {
        // +-----------------------------------------------------------+
        // |                   Latin Component Table                   |
        // +---+---------------+-------------------+-------------------+
        // |   | Ones     (--) | (--) Tens         | (--) Hundreds     |
        // +---+---------------+-------------------+-------------------+
        // | 1 | un            |  (n) deci         | (nx) centi        |
        // | 2 | duo           | (ms) viginti      |  (n) ducenti      |
        // | 3 | tre       (*) | (ns) triginta     | (ns) trecenti     |
        // | 4 | quattuor      | (ns) quadraginta  | (ns) quadringenti |
        // | 5 | quin          | (ns) quinquaginta | (ns) quingenti    |
        // | 6 | se       (sx) |  (n) sexaginta    |  (n) sescenti     |
        // | 7 | septe    (mn) |  (n) septuaginta  |  (n) septingenti  |
        // | 8 | octo          | (mx) octoginta    | (mx) octingenti   |
        // | 9 | nove     (mn) |      nonaginta    |      nongenti     |
        // +---+---------------+-------------------+-------------------+
        // Ref: http://www.mrob.com/pub/math/largenum.html#conway-wechsler

        [Flags] private enum ComponentFlag
        {
            None = 0x0,
            M    = 0x1,
            N    = 0x2,
            S    = 0x4,
            X    = 0x8
        }

        private enum ComponentFlagPosition
        {
            Prefix,
            Suffix
        }

        private record LatinComponent(string Component, ComponentFlag Flag, ComponentFlagPosition FlagPosition);

        private static readonly LatinComponent[] LatinOnes = new LatinComponent[] {
            new( "un",              ComponentFlag.None,                ComponentFlagPosition.Suffix ),
            new( "duo",             ComponentFlag.None,                ComponentFlagPosition.Suffix ),
            new( "tre",             ComponentFlag.None,                ComponentFlagPosition.Suffix ),
            new( "quattuor",        ComponentFlag.None,                ComponentFlagPosition.Suffix ),
            new( "quin",            ComponentFlag.None,                ComponentFlagPosition.Suffix ),
            new( "se",              ComponentFlag.S | ComponentFlag.X, ComponentFlagPosition.Suffix ),
            new( "septe",           ComponentFlag.M | ComponentFlag.N, ComponentFlagPosition.Suffix ),
            new( "octo",            ComponentFlag.None,                ComponentFlagPosition.Suffix ),
            new( "nove",            ComponentFlag.M | ComponentFlag.N, ComponentFlagPosition.Suffix )
        };

        private static readonly LatinComponent[] LatinTens = new LatinComponent[] {
            new( "deci",            ComponentFlag.N,                    ComponentFlagPosition.Prefix ),
            new( "viginti",         ComponentFlag.M | ComponentFlag.S,  ComponentFlagPosition.Prefix ),
            new( "triginta",        ComponentFlag.N | ComponentFlag.S,  ComponentFlagPosition.Prefix ),
            new( "quadraginta",     ComponentFlag.N | ComponentFlag.S,  ComponentFlagPosition.Prefix ),
            new( "quinquaginta",    ComponentFlag.N | ComponentFlag.S,  ComponentFlagPosition.Prefix ),
            new( "sexaginta",       ComponentFlag.N,                    ComponentFlagPosition.Prefix ),
            new( "septuaginta",     ComponentFlag.N,                    ComponentFlagPosition.Prefix ),
            new( "octoginta",       ComponentFlag.M | ComponentFlag.X,  ComponentFlagPosition.Prefix ),
            new( "nonaginta",       ComponentFlag.None,                 ComponentFlagPosition.Prefix )
        };

        private static readonly LatinComponent[] LatinHundreds = new LatinComponent[] {
            new( "centi",           ComponentFlag.N | ComponentFlag.X,  ComponentFlagPosition.Prefix ),
            new( "ducenti",         ComponentFlag.N,                    ComponentFlagPosition.Prefix ),
            new( "trecenti",        ComponentFlag.N | ComponentFlag.S,  ComponentFlagPosition.Prefix ),
            new( "quadringenti",    ComponentFlag.N | ComponentFlag.S,  ComponentFlagPosition.Prefix ),
            new( "quingenti",       ComponentFlag.N | ComponentFlag.S,  ComponentFlagPosition.Prefix ),
            new( "sescenti",        ComponentFlag.N,                    ComponentFlagPosition.Prefix ),
            new( "septingenti",     ComponentFlag.N,                    ComponentFlagPosition.Prefix ),
            new( "octingenti",      ComponentFlag.M | ComponentFlag.X,  ComponentFlagPosition.Prefix ),
            new( "nongenti",        ComponentFlag.None,                 ComponentFlagPosition.Prefix )
        };

        private const string Vowels = "aeio";

        // Rules:
        // - Break the provided number (Segment - 1) up into 1's, 10's and 100's
        //   Find the appropriate latin components for each
        // 
        // - Join the components, inserting an extra letter if there is a matching letter in parenthesis
        //   between a 1's and 10's or 100's component
        //
        // - If the 1's component is "tre," insert an "s" if the following component
        //   has either an s or an x in parenthesis.
        //
        // - If the combined components end with a vowel, remove that vowel
        //
        // - Add illion to the end
        //
        //  Examples:
        //     10^84 =   (84-3)/3 =  27 = (1s: 7 - septe(mn)) + (10s - 2 -(ms)viginti) = septe(m)viginti = septemvigintillion = (m)'s match;
        //    10^261 =  (261-3)/3 =  86 = (1s: 6 - se(sx)) + (10s: 8 - (mx)octoginta) = se(x)octoginta = sexoctogintillion
        //   10^2421 = (2421-3)/3 = 806 = (1s: 6 - se(sx)) + (100s: 8 - (mx)octingenti) = se(x)octingenti = sexoctingentillion

        // This class uses segments generated from the NumericSegmentEnumerator, where nonillion starts at the value of 10.
        //  0 - hundred
        //  1 - thousand
        //  2 - million
        // 10 - nonillion         - Final value before using this class
        // 11 - decillion         - 10: (1s: -) + (10s: deci) + -illion = dec-illion
        // 12 - undecillion       - 11: (1s: un) + (10s: deci) + -illion = un-dec-illion
        // 13 - duodecillion      - 12: (1s: duo) + (10s: deci) + -illion = duo-dec-illion

        public static string GetName(int segmentNumber)
        {
            if (segmentNumber < 11 || segmentNumber > 1000)
            {
                throw new ArgumentOutOfRangeException("segmentNumber", "Segment must be between 11-1000");
            }
            
            var component = SplitSegmentNumber(segmentNumber);

            var componentList = new List<LatinComponent>();
            if (component.Ones > 0) { componentList.Add(LatinOnes[component.Ones-1]); }
            if (component.Tens > 0) { componentList.Add(LatinTens[component.Tens-1]); }
            if (component.Hundreds > 0) { componentList.Add(LatinHundreds[component.Hundreds-1]); }

            var output = new List<string>();
            for (int i = 0; i < componentList.Count; i++)
            {
                var current = componentList[i];
                if (i == 1 && componentList[0].FlagPosition == ComponentFlagPosition.Suffix)
                {
                    var previous = componentList[0];
                    var matchedFlag = previous.Flag & current.Flag;

                    if (previous.Component == "tre" &&
                        (current.Flag & (ComponentFlag.S | ComponentFlag.X)) != ComponentFlag.None)
                    {
                        output.Add("s");
                    }
                    else if (matchedFlag != ComponentFlag.None)
                    {
                        switch (matchedFlag)
                        {
                            case ComponentFlag.M: output.Add("m"); break;
                            case ComponentFlag.N: output.Add("n"); break;
                            case ComponentFlag.S: output.Add("s"); break;
                            case ComponentFlag.X: output.Add("x"); break;
                        }
                    }
                }
                output.Add(current.Component);
            }

            var lastOutputEntry = output[output.Count-1];
            if (Vowels.Contains(lastOutputEntry[lastOutputEntry.Length-1]))
            {
                lastOutputEntry = lastOutputEntry.Substring(0, lastOutputEntry.Length - 1);
                output[output.Count-1] = lastOutputEntry;
            }

            output.Add("illion");
            return string.Join(string.Empty, output);
        }

        private static (int Hundreds, int Tens, int Ones) SplitSegmentNumber(int segmentNumber)
        {
            int hundreds = 0, tens = 0, ones = 0;
            int segment = segmentNumber - 1, remainder = 0;
            
            if (segment > 99)
            {
                remainder = segment % 100;
                hundreds = (segment - remainder) / 100;
                segment = remainder;
            }
            if (segment > 9)
            {
                remainder = segment % 10;
                tens = (segment - remainder) / 10;
                segment = remainder;
            }
            if (segment > 0)
            {
                ones = segment;
            }

            return (hundreds, tens, ones);
        }
    }
}