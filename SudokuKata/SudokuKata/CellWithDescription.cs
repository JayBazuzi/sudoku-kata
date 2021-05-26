using System;
using System.Collections.Generic;

namespace SudokuKata
{
    public class CellWithDescription
    {
        public CellWithDescription(Func<Cell, string> getDescription, Cell cell)
        {
            Cell = cell;
            Description = getDescription(cell);
        }

        public Cell Cell { get; }

        public string Description { get; set; }
        public int Index
        {
            get => Cell.ToIndex();
        }

        public int Row
        {
            get => Cell.Row;
        }

        public int Column
        {
            get => Cell.Column;
        }

        public override string ToString()
        {
            return
                $"{{ Description = {Description}, Index = {Index}, Row = {Row}, Column = {Column} }}";
        }
    }
}