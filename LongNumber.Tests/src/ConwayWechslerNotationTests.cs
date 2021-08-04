using System;
using Xunit;

namespace LongNumber.Tests
{
    public class ConwayWechslerNotationTests
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(10)]
        [InlineData(1001)]
        public void Validate_GetName_OutOfRangeThrows(int segmentNumber)
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => { _ = ConwayWechslerNotation.GetName(segmentNumber); });
            Assert.Equal("segmentNumber", exception.ParamName);
            Assert.Equal("Segment must be between 11-1000 (Parameter 'segmentNumber')", exception.Message);            
        }

        [Theory]
        [MemberData(nameof(SegmentNumberData))]
        public void Validate_GetName_SegmentNumberMatchesName(int segmentNumber, string expectedResult)
        {
            var result = ConwayWechslerNotation.GetName(segmentNumber);
            Assert.Equal(expectedResult, result);
        }

        public static TheoryData<int, string> SegmentNumberData => new TheoryData<int, string>
        {
            {  10 + 1, "decillion"                             }, // first value
            {  11 + 1, "undecillion"                           },
            {  12 + 1, "duodecillion"                          },
            {  27 + 1, "septemvigintillion"                    }, // ones + (m) case
            {  36 + 1, "sestrigintillion"                      }, // ones + (s) case
            {  39 + 1, "noventrigintillion"                    }, // ones + (n) case
            {  53 + 1, "tresquinquagintillion"                 }, //  tre + (s) special case
            {  86 + 1, "sexoctogintillion"                     }, // ones + (x) case
            { 454 + 1, "quattuorquinquagintaquadringentillion" }, // longest notation
            { 806 + 1, "sexoctingentillion"                    },
            { 999 + 1, "novenonagintanongentillion"            }  // last value
        };
    }
}