﻿using System.Collections.Generic;

namespace SudokuKata
{
    public class LookupStructures
    {
        public static readonly LookupStructures Instance = new LookupStructures();
        public Dictionary<int, int> _maskToOnesCount;
        public Dictionary<int, int> _singleBitToIndex;

        public LookupStructures()
        {
            var maskToOnesCount = new Dictionary<int, int>();
            maskToOnesCount[0] = 0;
            for (var i = 1; i < 1 << 9; i++)
            {
                var smaller = i >> 1;
                var increment = i & 1;
                maskToOnesCount[i] = maskToOnesCount[smaller] + increment;
            }

            var singleBitToIndex = new Dictionary<int, int>();
            for (var i = 0; i < 9; i++)
            {
                singleBitToIndex[1 << i] = i;
            }

            _singleBitToIndex = singleBitToIndex;
            _maskToOnesCount = maskToOnesCount;
        }
    }
}