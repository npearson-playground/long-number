using System.Collections.Generic;
using Xunit;

namespace LongNumber.Tests
{
    public class NumericSegmentEnumeratorTests
    {
        public record EnumeratorResult(int Segment, int Hundreds, int Tens, int Ones);

        [Fact]
        public void Validate_NumericSegmentEnumerator_EmptyInput()
        {
            var result = GetResultFromInputValue(string.Empty);
            Assert.True(result.Length == 0);
        }

        [Theory]
        [MemberData(nameof(NumericSegmentEnumeratorData))]
        public void Validate_NumericSegmentEnumerator_InputMatches(string input, EnumeratorResult[] expectedResult)
        {
            var result = GetResultFromInputValue(input);
            Assert.True(EnumeratorResultsAreEqual(result, expectedResult));
        }

        public static TheoryData<string, EnumeratorResult[]> NumericSegmentEnumeratorData => new TheoryData<string, EnumeratorResult[]>
        {
            {       "1", new EnumeratorResult[] { new EnumeratorResult(0, 0, 0, 1)  } },

            {      "25", new EnumeratorResult[] { new EnumeratorResult(0, 0, 2, 5)  } },

            {     "715", new EnumeratorResult[] { new EnumeratorResult(0, 7, 1, 5)  } },

            {    "8040", new EnumeratorResult[] { new EnumeratorResult(1, 0, 0, 8),
                                                  new EnumeratorResult(0, 0, 4, 0)  } },
                                                  
            { "1000500", new EnumeratorResult[] { new EnumeratorResult(2, 0, 0, 1),
                                                  new EnumeratorResult(1, 0, 0, 0),
                                                  new EnumeratorResult(0, 5, 0, 0)  } }
        };

        private EnumeratorResult[] GetResultFromInputValue(string input)
        {
            var segments = new List<EnumeratorResult>();
            foreach (var segment in new NumericSegmentEnumerator(input))
            {
                segments.Add(new EnumeratorResult(segment.Segment, segment.Hundreds, segment.Tens, segment.Ones));
            }
            return segments.ToArray();
        }

        private bool EnumeratorResultsAreEqual(EnumeratorResult[] left, EnumeratorResult[] right)
        {
            if (left.Length != right.Length) return false;
            for (int i = 0; i < left.Length; i++)
            {
                if (left[i] != right[i]) return false;
            }
            return true;
        }
    }

}
