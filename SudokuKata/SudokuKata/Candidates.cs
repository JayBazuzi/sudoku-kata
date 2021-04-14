using System.Collections.Generic;
using System.Linq;

namespace SudokuKata
{
    public class Candidates
    {
        static readonly LookupStructures _lookupStructures = Program.PrepareLookupStructures();
        
            public Candidates(int[] board)
        {
            Board = board;
        }

        public int[] Board { get; private set; }

        public Cell[] GetCellsWithOnlyOneCandidateRemaining()
        {
            var singleCandidateIndices =
                Board
                    .Select((mask, index) => new
                    {
                        CandidatesCount = _lookupStructures._maskToOnesCount[mask],
                        Index = index,
                        candidateMask = mask,
                        candidate = _lookupStructures._maskToOnesCount[mask] == 1
                            ? _lookupStructures._singleBitToIndex[mask]
                            : 0,
                    })
                    .Where(tuple => tuple.CandidatesCount == 1)
                    .Select(tuple => Cell.FromIndex(tuple.Index, tuple.candidate + 1))
                    .ToArray();
            return singleCandidateIndices;
        }
    }
}