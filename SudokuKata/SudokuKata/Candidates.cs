using System.Linq;

namespace SudokuKata
{
    public class Candidates
    {
        private static readonly LookupStructures _lookupStructures = Program.PrepareLookupStructures();

        public Candidates(int[] board)
        {
            Board = board;
        }

        public int[] Board { get; }

        public Cell[] GetCellsWithOnlyOneCandidateRemaining()
        {
            return Board
                .Select((mask, index) => new
                {
                    CandidatesCount = _lookupStructures._maskToOnesCount[mask],
                    Index = index,
                    candidateMask = mask
                })
                .Where(tuple => tuple.CandidatesCount == 1)
                .Select(tuple => new
                {
                    tuple.Index,
                    candidate = _lookupStructures._singleBitToIndex[tuple.candidateMask]
                })
                .Select(tuple => Cell.FromIndex(tuple.Index, tuple.candidate + 1))
                .ToArray();
        }
    }
}