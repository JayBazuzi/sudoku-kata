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
    }
}