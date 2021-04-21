﻿using System.Collections.Generic;
using System.Linq;

namespace SudokuKata
{
    public class Applesauce3
    {
        public int Mask { get; set; }
        public string Description { get; set; }
        public IGrouping<int, SudokuConstraints_OrSomething> Cells { get; set; }
        public List<SudokuConstraints_OrSomething> CellsWithMask { get; set; }
        public int CleanableCellsCount { get; set; }

        public override string ToString()
        {
            return
                $"{{ Mask = {Mask}, Description = {Description}, Cells = {Cells}, CellsWithMask = {CellsWithMask}, CleanableCellsCount = {CleanableCellsCount} }}";
        }

        public override bool Equals(object value)
        {
            var type = value as Applesauce3;
            return type != null && EqualityComparer<int>.Default.Equals(type.Mask, Mask) &&
                   EqualityComparer<string>.Default.Equals(type.Description, Description) &&
                   EqualityComparer<IGrouping<int, SudokuConstraints_OrSomething>>.Default.Equals(type.Cells, Cells) &&
                   EqualityComparer<List<SudokuConstraints_OrSomething>>.Default.Equals(type.CellsWithMask,
                       CellsWithMask) &&
                   EqualityComparer<int>.Default.Equals(type.CleanableCellsCount, CleanableCellsCount);
        }

        public override int GetHashCode()
        {
            var num = 0x7a2f0b42;
            num = -1521134295 * num + EqualityComparer<int>.Default.GetHashCode(Mask);
            num = -1521134295 * num + EqualityComparer<string>.Default.GetHashCode(Description);
            num = -1521134295 * num +
                  EqualityComparer<IGrouping<int, SudokuConstraints_OrSomething>>.Default.GetHashCode(Cells);
            num = -1521134295 * num +
                  EqualityComparer<List<SudokuConstraints_OrSomething>>.Default.GetHashCode(CellsWithMask);
            return -1521134295 * num + EqualityComparer<int>.Default.GetHashCode(CleanableCellsCount);
        }
    }
}