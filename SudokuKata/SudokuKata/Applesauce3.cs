using System.Collections.Generic;
using System.Linq;

namespace SudokuKata
{
    public class Applesauce3
    {
        public int Mask { get; set; }
        public string Description { get; set; }
        public IGrouping<int, Applesauce1> Cells { get; set; }
        public List<Applesauce1> CellsWithMask { get; set; }
        public int CleanableCellsCount { get; set; }

        public override string ToString()
        {
            return $"{{ Mask = {Mask}, Description = {Description}, Cells = {Cells}, CellsWithMask = {CellsWithMask}, CleanableCellsCount = {CleanableCellsCount} }}";
        }

        public override bool Equals(object value)
        {
            var type = value as Applesauce3;
            return (type != null) && EqualityComparer<int>.Default.Equals(type.Mask, Mask) && EqualityComparer<string>.Default.Equals(type.Description, Description) && EqualityComparer<IGrouping<int, Applesauce1>>.Default.Equals(type.Cells, Cells) && EqualityComparer<List<Applesauce1>>.Default.Equals(type.CellsWithMask, CellsWithMask) && EqualityComparer<int>.Default.Equals(type.CleanableCellsCount, CleanableCellsCount);
        }

        public override int GetHashCode()
        {
            int num = 0x7a2f0b42;
            num = (-1521134295 * num) + EqualityComparer<int>.Default.GetHashCode(Mask);
            num = (-1521134295 * num) + EqualityComparer<string>.Default.GetHashCode(Description);
            num = (-1521134295 * num) + EqualityComparer<IGrouping<int, Applesauce1>>.Default.GetHashCode(Cells);
            num = (-1521134295 * num) + EqualityComparer<List<Applesauce1>>.Default.GetHashCode(CellsWithMask);
            return (-1521134295 * num) + EqualityComparer<int>.Default.GetHashCode(CleanableCellsCount);
        }
    }
}