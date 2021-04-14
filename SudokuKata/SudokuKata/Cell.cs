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

        public static Cell FromIndex(int index)
        {
            var row = index / 9;
            var col = index % 9;
            var cell = new Cell(row, col);
            return cell;
        }

        public int ToIndex()
        {
            return Row * 9 + Col;
        }
    }
}