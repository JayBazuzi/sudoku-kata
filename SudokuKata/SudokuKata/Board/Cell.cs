using System.Collections.Generic;
using System.Linq;

namespace SudokuKata.Board
{
    public class Cell
    {
        public readonly int Column;
        public readonly int Row;
        public readonly int Value;

        public Cell(int row, int col, int value = 0)
        {
            Row = row;
            Column = col;
            Value = value;
        }

        public static Cell FromIndex(int index, int value = 0)
        {
            var row = index / 9;
            var col = index % 9;
            var cell = new Cell(row, col, value);
            return cell;
        }

        public int ToIndex()
        {
            return Row * 9 + Column;
        }

        public int Block
        {
            get
            {
                return (3 * (Row / 3)) + (Column / 3);
            }
        }

        public override string ToString()
        {
            return $"({Row}, {Column})";
        }

        public Cell WithValue(int value)
        {
            return new Cell(Row, Column, value);
        }

        public static IEnumerable<Cell> ForBoard()
        {
            return Enumerable.Range(0, 81).Select(FromIndex);
        }

        public bool IsCellBlock(Cell cell2)
        {
            return Block == cell2.Block;
        }

        public bool IsSameColumn(Cell cell2)
        {
            return Column == cell2.Column;
        }

        public bool IsSameRow(Cell cell2)
        {
            return Row == cell2.Row;
        }
    }
}