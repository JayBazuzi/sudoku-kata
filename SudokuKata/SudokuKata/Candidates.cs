namespace SudokuKata
{
    public class Candidates
    {
        public Candidates(int[] board)
        {
            Masks = board;
        }

        public int[] Masks { get; }
    }
}