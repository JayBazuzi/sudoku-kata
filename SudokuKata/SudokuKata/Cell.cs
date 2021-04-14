namespace SudokuKata
{
    public class Cell
    {
        public readonly int Row;
        public readonly int Col;

        public Cell(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public static Cell FromIndex(int singleCandidateIndex)
        {
            var row = singleCandidateIndex / 9;
            var col = singleCandidateIndex % 9;
            var cell = new Cell(row, col);
            return cell;
        }
    }
}