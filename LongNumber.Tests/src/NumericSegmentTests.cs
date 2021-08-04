using System;
using Xunit;

namespace LongNumber.Tests
{
    public class NumericSegmentTests
    {
        [Fact]
        public void Validate_NumericSegment_EmptyInputThrows()
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => { new NumericSegment(0, string.Empty); });
            Assert.Equal("input", exception.ParamName);
            Assert.Equal("NumericSegment must only contain one to three characters (Parameter 'input')", exception.Message);
        }

        [Fact]
        public void Validate_NumericSegment_LongInputThrows()
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => { new NumericSegment(0, "1234"); });
            Assert.Equal("input", exception.ParamName);
            Assert.Equal("NumericSegment must only contain one to three characters (Parameter 'input')", exception.Message);
        }

        [Fact]
        public void Validate_NumericSegment_NonNumericInputThrows()
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => { new NumericSegment(0, "abc"); });
            Assert.Equal("input", exception.ParamName);
            Assert.Equal("NumericSegment must only contain numeric characters (Parameter 'input')", exception.Message);
        }

        [Theory]
        [MemberData(nameof(NumericSegmentInputData))]
        public void Validate_NumericSegment_InputMatches(int segmentNumber, string numericString, string expectedResult)
        {
            var segment = new NumericSegment(segmentNumber, numericString);
            var result = segment.ToString();
            Assert.Equal(expectedResult, result);
        }

        public static TheoryData<int, string, string> NumericSegmentInputData => new TheoryData<int, string, string>
        {
            {  0,   "0", string.Empty                           },
            {  0,  "19", "nineteen"                             },
            {  0,  "20", "twenty"                               },
            {  0,  "43", "fourty-three"                         },
            {  0, "100", "one hundred"                          },
            {  0, "123", "one hundred twenty-three"             },
            {  0, "530", "five hundred thirty"                  },
            {  1,  "55", "fifty-five thousand"                  },
            {  2, "888", "eight hundred eighty-eight million"   },
            {  3,  "30", "thirty billion"                       },
            {  4,   "1", "one trillion"                         },
            { 10,  "10", "ten nonillion"                        },
            { 11,  "11", "eleven decillion"                     },
            { 19,  "18", "eighteen octodecillion"               }
        };
    }
}
