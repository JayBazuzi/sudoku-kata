using System.Collections.Generic;

namespace SudokuKata.Board
{
    public class CellWithPossiblities
    {
        public readonly List<int> Possibilities;
        public readonly Cell Cell;

        public CellWithPossiblities(int index, List<int> possibilities)
        {
            this.Possibilities = possibilities;
            this.Cell = Cell.FromIndex(index);
        }
    }
}