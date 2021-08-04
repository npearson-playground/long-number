using System;

namespace LongNumber
{
    internal ref struct NumericSegmentEnumerator
    {
        private int nextSegmentNumber;
        private ReadOnlySpan<char> nextSegment;
        private ReadOnlySpan<char> remaining;

        public NumericSegment Current { get; private set; }

        public NumericSegmentEnumerator(ReadOnlySpan<char> input)
        {
            if (input.Length == 0)
            {
                nextSegmentNumber = 0;
                nextSegment = ReadOnlySpan<char>.Empty;
                remaining = ReadOnlySpan<char>.Empty;
            }
            else
            {
                int partialSegmentLength = input.Length % 3;
                if (partialSegmentLength != 0)
                {
                    nextSegment = input.Slice(0, partialSegmentLength);
                    remaining = input.Slice(partialSegmentLength);
                }
                else
                {
                    nextSegment = input.Slice(0, 3);
                    remaining = input.Slice(3);
                }
            }

            nextSegmentNumber = remaining.Length / 3;
            Current = default;
        }

        public NumericSegmentEnumerator GetEnumerator() => this;

        public bool MoveNext()
        {
            if (nextSegment.Length == 0) { return false; }

            Current = new NumericSegment(nextSegmentNumber, nextSegment);
            nextSegmentNumber -= 1;

            if (remaining.Length > 0)
            {
                nextSegment = remaining.Slice(0, 3);
                remaining = remaining.Slice(3);
            }
            else
            {
                nextSegment = ReadOnlySpan<char>.Empty;
            }
            
            return true;
        }
    }
}