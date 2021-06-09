using System.Linq;

namespace SudokuKata
{
    public class Candidates
    {
        private static readonly LookupStructures _lookupStructures = LookupStructures.Instance;

        public Candidates(int[] board)
        {
            Masks = board;
        }

        public int[] Masks { get; }

        public Cell[] GetCellsWithOnlyOneCandidateRemaining()
        {
            return Masks
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