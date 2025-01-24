namespace Core.Shared;

public static class CycleFinder
{
    /// <summary>
    /// Finds the repeating cycle in the input sequence, if any. Assumes logic is deterministic and produces a repeating cycle.
    /// </summary>
    public static CycleAnalysis<T> FindCycle<T>(IEnumerable<T> inputCycle, long maxIterations = long.MaxValue)
        where T : IEquatable<T>
    {
        var valuesSeenInOrder = new List<T>();
        var valuesSeenSet = new HashSet<T>(); // faster for lookup to see if we've seen a value before

        using var inputEnumerator = inputCycle.GetEnumerator();
        
        for (int i = 0; i < maxIterations; i++)
        {
            inputEnumerator.MoveNext();
            var current = inputEnumerator.Current;

            var seenBefore = valuesSeenSet.Contains(current);
            
            if (seenBefore)
            {
                var seenAtIndex = valuesSeenInOrder.IndexOf(current);
                var cycleStartIndex = seenAtIndex;
                var cycleLength = (i - seenAtIndex);
                return new CycleAnalysis<T>(cycleStartIndex, cycleLength, valuesSeenInOrder);
            }

            valuesSeenInOrder.Add(current);
            valuesSeenSet.Add(current);
        }

        throw new InvalidOperationException(
            $"No cycle found in input sequence after checking {maxIterations} iterations.");
    }

    public class CycleAnalysis<T>
    {
        public int CycleStartIndex { get; }
        public int CycleLength { get; }
        public List<T> ValuesSeen { get; }

        public CycleAnalysis(int cycleStartIndex, int cycleLength, List<T> valuesSeen)
        {
            CycleStartIndex = cycleStartIndex;
            CycleLength = cycleLength;
            ValuesSeen = valuesSeen;
        }
        
        public T FindValueAt(long targetIndex)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(targetIndex, nameof(targetIndex));
            var indexWithinCycle = (targetIndex - CycleStartIndex) % CycleLength;
            var indexWithinSeenValues = CycleStartIndex + (int)indexWithinCycle;
            var valueAtTargetIndex = ValuesSeen[indexWithinSeenValues];
            return valueAtTargetIndex;
        }
    }
}