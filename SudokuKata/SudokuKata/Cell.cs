namespace SudokuKata
{
    public class Cell
    {
        public readonly int Col;
        public readonly int Row;
        public readonly int Value;

        public Cell(int row, int col, int value)
        {
            Row = row;
            Col = col;
            Value = value;
        }

        public static Cell FromIndex(int index, int value)
        {
            var row = index / 9;
            var col = index % 9;
            var cell = new Cell(row, col, value);
            return cell;
        }

        public int ToIndex()
        {
            return Row * 9 + Col;
        }

        public override string ToString()
        {
            return $"({Row}, {Col})";
        }
    }
}