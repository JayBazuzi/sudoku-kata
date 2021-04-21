using System.Collections.Generic;

namespace SudokuKata
{
    public class LookupStructures
    {
        public Dictionary<int, int> _maskToOnesCount;
        public Dictionary<int, int> _singleBitToIndex;

        public LookupStructures(Dictionary<int, int> singleBitToIndex,
            Dictionary<int, int> maskToOnesCount)
        {
            _singleBitToIndex = singleBitToIndex;
            _maskToOnesCount = maskToOnesCount;
        }
    }
}