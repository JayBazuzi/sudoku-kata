namespace SudokuKata
{
    public class Candidates
    {
        public Candidates(int[] board)
        {
            Board = board;
        }

        public int[] Board { get; private set; }
    }
}