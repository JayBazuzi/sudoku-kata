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

        public int[] GetCellsWithOnlyOneCandidateRemaining()
        {
            var singleCandidateIndices =
                Board
                    .Select((mask, index) => new
                    {
                        CandidatesCount = _lookupStructures._maskToOnesCount[mask],
                        Index = index
                    })
                    .Where(tuple => tuple.CandidatesCount == 1)
                    .Select(tuple => tuple.Index)
                    .ToArray();
            return singleCandidateIndices;
        }
    }
}