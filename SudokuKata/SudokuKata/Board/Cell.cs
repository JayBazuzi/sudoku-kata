﻿using System.Collections.Generic;

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
            for (var j = 0; j < 81; j++)
            {
                yield return Cell.FromIndex(j);
            }
        }
    }
}