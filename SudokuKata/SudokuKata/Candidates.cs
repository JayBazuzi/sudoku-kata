using System.Collections.Generic;
using System.Linq;

namespace SudokuKata
{
    public class Candidates
    {
        public Candidates(int[] board)
        {
            Board = board;
        }

        public int[] Board { get; private set; }

        public int[] GetCellsWithOnlyOneCandidateRemaining(Dictionary<int, int> maskToOnesCount)
        {
            var singleCandidateIndices =
                Board
                    .Select((mask, index) => new
                    {
                        CandidatesCount = maskToOnesCount[mask],
                        Index = index
                    })
                    .Where(tuple => tuple.CandidatesCount == 1)
                    .Select(tuple => tuple.Index)
                    .ToArray();
            return singleCandidateIndices;
        }
    }
}